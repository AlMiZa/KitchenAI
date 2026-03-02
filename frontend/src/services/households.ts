import { apiFetch } from './api';

export interface Household {
  id: string;
  name: string;
  memberCount: number;
}

export interface HouseholdMember {
  userId: string;
  displayName: string;
  email: string;
  role: 'owner' | 'member';
}

export const getHousehold = (householdId: string) =>
  apiFetch<Household>(`/households/${householdId}`);

export const getHouseholds = () => apiFetch<Household[]>('/households');

export const getHouseholdMembers = (householdId: string) =>
  apiFetch<HouseholdMember[]>(`/households/${householdId}/members`);

export const getInviteLink = (householdId: string) =>
  apiFetch<{ inviteLink: string }>(`/households/${householdId}/invite`);

export const leaveHousehold = (householdId: string) =>
  apiFetch<void>(`/households/${householdId}/leave`, { method: 'POST' });
