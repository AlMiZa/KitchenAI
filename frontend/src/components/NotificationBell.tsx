import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../hooks/useAuth';
import {
  getNotifications,
  markAllRead,
} from '../services/notifications';
import type { AppNotification } from '../services/notifications';
import { formatDateTime } from '../utils/dateFormat';

function typeIcon(type: AppNotification['type']): string {
  switch (type) {
    case 'expiry_warning': return '⚠️';
    case 'recipe_ready':   return '🍳';
    case 'shopping_alert': return '🛒';
    default:               return 'ℹ️';
  }
}

/** Bell icon with unread-count badge + dropdown panel. Polls every 30 s. */
export default function NotificationBell() {
  const { t, i18n } = useTranslation();
  const { householdId } = useAuth();
  const queryClient = useQueryClient();
  const [open, setOpen] = useState(false);

  const { data: notifications = [] } = useQuery({
    queryKey: ['notifications', householdId],
    queryFn: () => getNotifications(householdId!),
    enabled: !!householdId,
    refetchInterval: 30_000,
  });

  const unreadCount = notifications.filter((n) => !n.read).length;

  const markReadMutation = useMutation({
    mutationFn: () => markAllRead(householdId!),
    onSuccess: () =>
      queryClient.invalidateQueries({ queryKey: ['notifications', householdId] }),
  });

  return (
    <div className="relative">
      <button
        onClick={() => setOpen((o) => !o)}
        aria-label={t('notifications.title')}
        className="relative p-2 rounded-full hover:bg-gray-100 text-gray-600 transition-colors"
      >
        {/* Bell SVG */}
        <svg
          className="w-6 h-6"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
          aria-hidden="true"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
            d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002
               6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6
               8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4
               17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"
          />
        </svg>

        {/* Unread badge */}
        {unreadCount > 0 && (
          <span
            className="absolute top-0 right-0 bg-red-500 text-white text-xs
                       rounded-full w-5 h-5 flex items-center justify-center font-bold"
            aria-label={`${unreadCount} ${t('notifications.unread')}`}
          >
            {unreadCount}
          </span>
        )}
      </button>

      {/* Dropdown panel */}
      {open && (
        <div
          className="absolute right-0 mt-2 w-80 bg-white rounded-xl shadow-xl
                     border border-gray-200 z-50 overflow-hidden"
          role="dialog"
          aria-modal="true"
          aria-label={t('notifications.title')}
        >
          <div className="flex items-center justify-between px-4 py-3 border-b border-gray-100">
            <h3 className="font-semibold text-gray-800 text-sm">
              {t('notifications.title')}
            </h3>
            <button
              onClick={() => markReadMutation.mutate()}
              className="text-xs text-green-600 hover:underline"
            >
              {t('notifications.markAllRead')}
            </button>
          </div>

          <ul className="max-h-72 overflow-y-auto divide-y divide-gray-50">
            {notifications.length === 0 ? (
              <li className="px-4 py-6 text-sm text-gray-400 text-center">
                {t('notifications.noNotifications')}
              </li>
            ) : (
              notifications.map((n) => (
                <li
                  key={n.id}
                  className={`px-4 py-3 flex gap-3 ${!n.read ? 'bg-green-50' : ''}`}
                >
                  <span className="text-lg leading-none">{typeIcon(n.type)}</span>
                  <div className="flex-1 min-w-0">
                    <p className="text-sm text-gray-800 break-words">{n.message}</p>
                    <p className="text-xs text-gray-400 mt-0.5">
                      {formatDateTime(n.createdAt, i18n.language)}
                    </p>
                  </div>
                </li>
              ))
            )}
          </ul>
        </div>
      )}
    </div>
  );
}
