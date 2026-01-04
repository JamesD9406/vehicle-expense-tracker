import { formatCurrency } from '../../utils/helpers';
import type { FuelEfficiencyDto } from '../../types/Fuel';

interface FuelEfficiencyMetricsProps {
  efficiency: FuelEfficiencyDto;
}

export function FuelEfficiencyMetrics({ efficiency }: FuelEfficiencyMetricsProps) {
  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      {efficiency.totalFuelLiters > 0 && (
        <>
          <EfficiencyCard
            title="Fuel Consumption"
            value={`${efficiency.averageLitersPer100Km.toFixed(2)} L/100km`}
            subtitle={`${efficiency.totalFuelLiters.toFixed(1)}L total`}
          />
          <EfficiencyCard
            title="Fuel Cost/km"
            value={formatCurrency(efficiency.averageFuelCostPerKm)}
            subtitle={`${formatCurrency(efficiency.totalFuelCost)} total`}
          />
        </>
      )}
      
      {efficiency.totalElectricityKwh > 0 && (
        <>
          <EfficiencyCard
            title="Electric Consumption"
            value={`${efficiency.averageKwhPer100Km.toFixed(2)} kWh/100km`}
            subtitle={`${efficiency.totalElectricityKwh.toFixed(1)}kWh total`}
          />
          <EfficiencyCard
            title="Electric Cost/km"
            value={formatCurrency(efficiency.averageElectricityCostPerKm)}
            subtitle={`${formatCurrency(efficiency.totalElectricityCost)} total`}
          />
        </>
      )}
      
      <EfficiencyCard
        title="Average Cost/km"
        value={formatCurrency(efficiency.averageCostPerKm)}
        subtitle={`${efficiency.totalKilometers.toFixed(0)}km tracked`}
        highlight
      />
      
      <EfficiencyCard
        title="Total Entries"
        value={`${efficiency.numberOfFuelEntries + efficiency.numberOfChargeEntries}`}
        subtitle={`${efficiency.numberOfFuelEntries} fuel, ${efficiency.numberOfChargeEntries} electric`}
      />
    </div>
  );
}

interface EfficiencyCardProps {
  title: string;
  value: string;
  subtitle: string;
  highlight?: boolean;
}

function EfficiencyCard({ title, value, subtitle, highlight }: EfficiencyCardProps) {
  return (
    <div className={`${highlight ? 'bg-green-600/20 border-green-500' : 'bg-gray-700/50'} border ${highlight ? '' : 'border-gray-600'} rounded-lg p-4`}>
      <p className="text-gray-300 text-sm">{title}</p>
      <p className={`${highlight ? 'text-green-400' : 'text-white'} text-lg font-bold mt-1`}>{value}</p>
      <p className="text-gray-400 text-xs mt-1">{subtitle}</p>
    </div>
  );
}
