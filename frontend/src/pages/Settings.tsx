import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../hooks/useAuth';
import { updateProfile, exportData, deleteAccount } from '../services/auth';
import { subscribeNotifications } from '../services/notifications';

type Tab = 'profile' | 'notifications' | 'household' | 'privacy';

export default function SettingsPage() {
  const { t } = useTranslation();
  const { user, householdId, logout } = useAuth();
  const [activeTab, setActiveTab] = useState<Tab>('profile');

  // --- Profile state ---
  const [displayName, setDisplayName] = useState(user?.displayName ?? '');
  const [locale, setLocale]           = useState(user?.locale ?? 'en');
  const [profileMsg, setProfileMsg]   = useState('');

  // --- Notifications state ---
  const [expiryDays, setExpiryDays]     = useState(3);
  const [emailOpt, setEmailOpt]         = useState(false);
  const [pushOpt, setPushOpt]           = useState(false);
  const [notifMsg, setNotifMsg]         = useState('');

  // --- Privacy state ---
  const [deleteConfirm, setDeleteConfirm] = useState(false);

  const profileMut = useMutation({
    mutationFn: () => updateProfile({ displayName, locale }),
    onSuccess: () => setProfileMsg(t('settings.profileSaved')),
  });

  const notifMut = useMutation({
    mutationFn: () =>
      subscribeNotifications(householdId!, {
        expiryThresholdDays: expiryDays,
        emailEnabled: emailOpt,
        pushEnabled: pushOpt,
      }),
    onSuccess: () => setNotifMsg(t('settings.notificationsSaved')),
  });

  const exportMut = useMutation({
    mutationFn: exportData,
    onSuccess: () => {
      // Trigger browser download with a placeholder link
      const a = document.createElement('a');
      a.href = '/api/users/me/export';
      a.download = 'my-data.json';
      a.click();
    },
  });

  const deleteMut = useMutation({
    mutationFn: deleteAccount,
    onSuccess: logout,
  });

  const tabs: { id: Tab; label: string }[] = [
    { id: 'profile',       label: t('settings.profile') },
    { id: 'notifications', label: t('settings.notifications') },
    { id: 'household',     label: t('settings.household') },
    { id: 'privacy',       label: t('settings.privacy') },
  ];

  return (
    <div>
      <h1 className="text-2xl font-semibold text-gray-800 mb-6">{t('pages.settings')}</h1>

      {/* Tab bar */}
      <div className="flex gap-1 mb-6 border-b border-gray-200">
        {tabs.map(({ id, label }) => (
          <button
            key={id}
            onClick={() => setActiveTab(id)}
            className={`px-4 py-2 text-sm font-medium border-b-2 transition-colors ${
              activeTab === id
                ? 'border-green-600 text-green-600'
                : 'border-transparent text-gray-500 hover:text-gray-700'
            }`}
          >
            {label}
          </button>
        ))}
      </div>

      <div className="max-w-lg">
        {/* Profile tab */}
        {activeTab === 'profile' && (
          <section className="space-y-4">
            <div>
              <label htmlFor="s-name" className="block text-sm font-medium text-gray-700 mb-1">
                {t('settings.displayName')}
              </label>
              <input
                id="s-name"
                type="text"
                value={displayName}
                onChange={(e) => setDisplayName(e.target.value)}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
              />
            </div>
            <div>
              <label htmlFor="s-email" className="block text-sm font-medium text-gray-700 mb-1">
                {t('settings.email')}
              </label>
              <input
                id="s-email"
                type="email"
                value={user?.email ?? ''}
                readOnly
                className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm bg-gray-50 text-gray-500"
              />
            </div>
            <div>
              <label htmlFor="s-locale" className="block text-sm font-medium text-gray-700 mb-1">
                {t('settings.locale')}
              </label>
              <select
                id="s-locale"
                value={locale}
                onChange={(e) => setLocale(e.target.value)}
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
              >
                <option value="en">English</option>
                <option value="pl">Polski</option>
              </select>
            </div>
            {profileMsg && <p className="text-green-600 text-sm">{profileMsg}</p>}
            <button
              onClick={() => profileMut.mutate()}
              disabled={profileMut.isPending}
              className="bg-green-600 hover:bg-green-700 disabled:opacity-50 text-white px-5 py-2 rounded-lg text-sm font-medium"
            >
              {t('settings.saveProfile')}
            </button>
          </section>
        )}

        {/* Notifications tab */}
        {activeTab === 'notifications' && (
          <section className="space-y-5">
            <div>
              <label htmlFor="s-days" className="block text-sm font-medium text-gray-700 mb-1">
                {t('settings.expiryThreshold')}
              </label>
              <input
                id="s-days"
                type="number"
                min="1"
                max="30"
                value={expiryDays}
                onChange={(e) => setExpiryDays(Number(e.target.value))}
                className="w-24 border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
              />
            </div>
            <label className="flex items-center gap-3 text-sm text-gray-700 cursor-pointer">
              <input
                type="checkbox"
                checked={emailOpt}
                onChange={(e) => setEmailOpt(e.target.checked)}
                className="w-4 h-4 rounded text-green-600"
              />
              {t('settings.emailNotifications')}
            </label>
            <label className="flex items-center gap-3 text-sm text-gray-700 cursor-pointer">
              <input
                type="checkbox"
                checked={pushOpt}
                onChange={(e) => setPushOpt(e.target.checked)}
                className="w-4 h-4 rounded text-green-600"
              />
              {t('settings.pushNotifications')}
            </label>
            {notifMsg && <p className="text-green-600 text-sm">{notifMsg}</p>}
            <button
              onClick={() => notifMut.mutate()}
              disabled={notifMut.isPending || !householdId}
              className="bg-green-600 hover:bg-green-700 disabled:opacity-50 text-white px-5 py-2 rounded-lg text-sm font-medium"
            >
              {t('settings.saveNotifications')}
            </button>
          </section>
        )}

        {/* Household tab */}
        {activeTab === 'household' && (
          <section className="space-y-4">
            <div>
              <p className="text-sm text-gray-500 mb-1">{t('settings.householdName')}</p>
              <p className="text-gray-800 font-medium">{householdId ?? '—'}</p>
            </div>
            <button
              disabled
              className="bg-gray-100 text-gray-400 px-5 py-2 rounded-lg text-sm font-medium cursor-not-allowed"
            >
              {t('settings.inviteMember')}
            </button>
          </section>
        )}

        {/* Privacy tab */}
        {activeTab === 'privacy' && (
          <section className="space-y-6">
            <div>
              <h3 className="text-sm font-medium text-gray-700 mb-2">{t('settings.exportData')}</h3>
              <button
                onClick={() => exportMut.mutate()}
                disabled={exportMut.isPending}
                className="bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white px-5 py-2 rounded-lg text-sm font-medium"
              >
                {t('settings.exportData')}
              </button>
            </div>

            <div className="border-t border-gray-200 pt-6">
              <h3 className="text-sm font-medium text-red-600 mb-2">{t('settings.deleteAccount')}</h3>
              {!deleteConfirm ? (
                <button
                  onClick={() => setDeleteConfirm(true)}
                  className="border border-red-500 text-red-600 hover:bg-red-50 px-5 py-2 rounded-lg text-sm font-medium"
                >
                  {t('settings.deleteAccount')}
                </button>
              ) : (
                <div className="space-y-3 bg-red-50 rounded-xl p-4">
                  <p className="text-sm text-red-700">{t('settings.deleteConfirm')}</p>
                  <div className="flex gap-3">
                    <button
                      onClick={() => deleteMut.mutate()}
                      disabled={deleteMut.isPending}
                      className="bg-red-600 hover:bg-red-700 disabled:opacity-50 text-white px-4 py-2 rounded-lg text-sm font-medium"
                    >
                      {t('common.confirm')}
                    </button>
                    <button
                      onClick={() => setDeleteConfirm(false)}
                      className="border border-gray-300 px-4 py-2 rounded-lg text-sm text-gray-600 hover:bg-gray-50"
                    >
                      {t('common.cancel')}
                    </button>
                  </div>
                </div>
              )}
            </div>
          </section>
        )}
      </div>
    </div>
  );
}
