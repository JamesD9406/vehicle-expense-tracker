import apiClient from './api';
import type { Vehicle } from '../types/Vehicle';

export const vehicleService = {
  // Get all user's vehicles
  async getAll(): Promise<Vehicle[]> {
    const response = await apiClient.get<Vehicle[]>('/vehicles');
    return response.data;
  },

  // Get specific vehicle
  async getById(id: number): Promise<Vehicle> {
    const response = await apiClient.get<Vehicle>(`/vehicles/${id}`);
    return response.data;
  },
};
