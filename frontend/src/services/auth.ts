import { apiFetch } from './api';

export interface AuthResult {
  token: string;
  userId: string;
  email: string;
  displayName: string;
  householdId: string | null;
}

export interface UserProfile {
  id: string;
  email: string;
  displayName: string;
  locale: string;
  householdId: string | null;
}

export const register = (data: { email: string; password: string; displayName: string }) =>
  apiFetch<AuthResult>('/auth/register', { method: 'POST', body: JSON.stringify(data) });

export const login = (data: { email: string; password: string }) =>
  apiFetch<AuthResult>('/auth/login', { method: 'POST', body: JSON.stringify(data) });

export const getMe = () => apiFetch<UserProfile>('/auth/me');

export const updateProfile = (data: Partial<Pick<UserProfile, 'displayName' | 'locale'>>) =>
  apiFetch<UserProfile>('/auth/me', { method: 'PUT', body: JSON.stringify(data) });

export const exportData = () => apiFetch<Blob>('/users/me/export');

export const deleteAccount = () => apiFetch<void>('/users/me', { method: 'DELETE' });
