import apiClient from './api';
import type { TcoReportDto, VehicleSummaryDto } from '../types/Report';

export const reportService = {
  // Get TCO report for a specific vehicle
  async getTcoReport(vehicleId: number): Promise<TcoReportDto> {
    const response = await apiClient.get<TcoReportDto>(`/reports/tco/${vehicleId}`);
    return response.data;
  },

  // Get summary across all user's vehicles
  async getSummary(): Promise<VehicleSummaryDto> {
    const response = await apiClient.get<VehicleSummaryDto>('/reports/summary');
    return response.data;
  },
};
