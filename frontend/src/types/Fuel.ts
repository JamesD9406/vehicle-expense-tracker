export const EnergyType = {
  Gasoline: 0,
  Diesel: 1,
  Electricity: 2,
} as const;

export type EnergyType = typeof EnergyType[keyof typeof EnergyType];

export interface FuelEntry {
  id: number;
  energyType: number;
  energyTypeDisplay: string;
  amount: number;
  unit: string;
  cost: number;
  odometer: number;
  date: string;
  vehicleId: number;
  vehicleMake: string;
  vehicleModel: string;
  vehicleType: number;
  costPerUnit: number;
}

export interface CreateFuelEntryDto {
  energyType: number;
  amount: number;
  cost: number;
  odometer: number;
  date: string;
  vehicleId: number;
}

export interface UpdateFuelEntryDto {
  energyType: number;
  amount: number;
  cost: number;
  odometer: number;
  date: string;
}

export interface FuelEfficiencyDto {
  vehicleId: number;
  vehicleMake: string;
  vehicleModel: string;
  vehicleType: number;
  totalFuelLiters: number;
  totalFuelCost: number;
  averageLitersPer100Km: number;
  averageFuelCostPerKm: number;
  totalElectricityKwh: number;
  totalElectricityCost: number;
  averageKwhPer100Km: number;
  averageElectricityCostPerKm: number;
  totalEnergyCost: number;
  averageCostPerKm: number;
  totalKilometers: number;
  firstEntryDate?: string;
  lastEntryDate?: string;
  numberOfFuelEntries: number;
  numberOfChargeEntries: number;
}
