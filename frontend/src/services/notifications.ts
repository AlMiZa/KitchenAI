import { apiFetch } from './api';

export interface AppNotification {
  id: string;
  type: 'expiry_warning' | 'recipe_ready' | 'shopping_alert' | 'info';
  message: string;
  read: boolean;
  createdAt: string;
}

export interface NotificationSubscription {
  expiryThresholdDays: number;
  emailEnabled: boolean;
  pushEnabled: boolean;
}

export const getNotifications = (householdId: string) =>
  apiFetch<AppNotification[]>(`/households/${householdId}/notifications`);

export const markAllRead = (householdId: string) =>
  apiFetch<void>(`/households/${householdId}/notifications/read-all`, { method: 'POST' });

export const subscribeNotifications = (householdId: string, data: NotificationSubscription) =>
  apiFetch<void>(`/households/${householdId}/notifications/subscribe`, {
    method: 'POST',
    body: JSON.stringify(data),
  });
