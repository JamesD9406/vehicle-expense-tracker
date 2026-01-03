import apiClient from './api';
import type { FuelEntry, CreateFuelEntryDto, UpdateFuelEntryDto, FuelEfficiencyDto } from '../types/Fuel';

export const fuelService = {
  async getAll(vehicleId?: number): Promise<FuelEntry[]> {
    const url = vehicleId ? `/fuel?vehicleId=${vehicleId}` : '/fuel';
    const response = await apiClient.get<FuelEntry[]>(url);
    return response.data;
  },

  async getById(id: number): Promise<FuelEntry> {
    const response = await apiClient.get<FuelEntry>(`/fuel/${id}`);
    return response.data;
  },

  async create(data: CreateFuelEntryDto): Promise<FuelEntry> {
    const response = await apiClient.post<FuelEntry>('/fuel', data);
    return response.data;
  },

  async update(id: number, data: UpdateFuelEntryDto): Promise<FuelEntry> {
    const response = await apiClient.put<FuelEntry>(`/fuel/${id}`, data);
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/fuel/${id}`);
  },

  async getEfficiency(vehicleId: number): Promise<FuelEfficiencyDto> {
    const response = await apiClient.get<FuelEfficiencyDto>(`/fuel/efficiency/${vehicleId}`);
    return response.data;
  },
};
