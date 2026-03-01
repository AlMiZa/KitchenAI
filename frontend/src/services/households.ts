import { apiFetch } from './api';

export interface Household {
  id: string;
  name: string;
  memberCount: number;
}

export const getHousehold = (householdId: string) =>
  apiFetch<Household>(`/households/${householdId}`);

export const getHouseholds = () => apiFetch<Household[]>('/households');
