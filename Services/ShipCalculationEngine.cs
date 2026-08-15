using System;
using System.Collections.Generic;
using AuroraDesignSuite.Models;

namespace AuroraDesignSuite.Services
{
    public static class FormulaCalculator
    {
        // Calculate Speed in km/s
        public static double CalculateSpeed(double totalEnginePower, double totalHS)
        {
            if (totalHS <= 0) return 0;
            return (totalEnginePower / totalHS) * 1000.0;
        }

        // Calculate Range in Billion Km
        public static double CalculateRangeBillionKm(double speedKmS, double totalFuelLiters, double fuelConsumptionLitersPerHour)
        {
            if (fuelConsumptionLitersPerHour <= 0 || totalFuelLiters <= 0) return 0;
            double flightHours = totalFuelLiters / fuelConsumptionLitersPerHour;
            double distanceKm = flightHours * speedKmS * 3600.0;
            return distanceKm / 1_000_000_000.0;
        }

        // Calculate Annual Failure Rate
        public static double CalculateAnnualFailureRate(double totalHS, double engineeringSpacesHS)
        {
            if (engineeringSpacesHS <= 0) engineeringSpacesHS = 0.1; // Baseline small chance
            double hsFactor = Math.Pow(totalHS / 100.0, 1.25);
            return hsFactor / engineeringSpacesHS;
        }

        // Calculate Maintenance Life in Years
        public static double CalculateMaintenanceLife(double totalMSP, double annualFailureRate, double totalCostBP)
        {
            if (annualFailureRate <= 0) return 99.0;
            double avgRepairCost = Math.Max(1.0, totalCostBP * 0.05);
            double years = totalMSP / (annualFailureRate * avgRepairCost);
            return Math.Min(99.0, Math.Round(years, 2));
        }

        // Missile Performance Calculator
        public static MissileBlueprint CalculateMissileMetrics(MissileBlueprint mb)
        {
            double totalSize = mb.EngineSizeHS + mb.WarheadMSP + mb.FuelHS + mb.AgilityHS;
            mb.MissileSizeHS = totalSize;

            double power = mb.EngineSizeHS * mb.EnginePowerModifier * 10.0;
            mb.SpeedKmS = (power / totalSize) * 1000.0;
            mb.WarheadDamage = Math.Round(mb.WarheadMSP * 4.0, 1);

            double fuelLit = mb.FuelHS * 2500.0;
            double fuelCons = power * 0.1;
            mb.RangeMillionKm = (fuelLit / Math.Max(0.01, fuelCons)) * (mb.SpeedKmS * 3600.0) / 1_000_000.0;

            double agilityRating = mb.AgilityHS * 10.0;
            double maneuveringFactor = (agilityRating * 1000.0) / totalSize;
            mb.HitChanceVs5000KmS = Math.Min(100.0, (mb.SpeedKmS / 5000.0) * 100.0 + maneuveringFactor);

            mb.TotalCostBP = Math.Round(totalSize * 1.5, 2);
            mb.Minerals = new MineralRequirement();
            mb.Minerals.Add("Gallicite", mb.TotalCostBP * 0.5);
            mb.Minerals.Add("Uridium", mb.TotalCostBP * 0.3);
            mb.Minerals.Add("Duranium", mb.TotalCostBP * 0.2);

            return mb;
        }
    }

    public class ShipCalculationEngine
    {
        public ShipDesign RecalculateDesign(ShipDesign design)
        {
            design.Warnings.Clear();
            design.Suggestions.Clear();

            double compHS = 0;
            double compCost = 0;
            int crewNeeded = 0;
            double enginePower = 0;
            double fuelCap = 0;
            double msp = 0;
            double engineeringHS = 0;
            double shieldStr = 0;
            double hangarCap = 0;
            double magCap = 0;
            int jumpSquadron = 0;
            int jumpMaxHS = 0;
            bool hasJumpDrive = false;
            int crewQuartersProvided = 0;
            bool isMilitaryComponentFound = false;

            var minerals = new MineralRequirement();

            foreach (var item in design.Components)
            {
                compHS += item.TotalHS;
                compCost += item.TotalCost;
                crewNeeded += item.TotalCrew;

                var comp = item.Component;
                string typeName = comp.TypeName.ToLower();
                string compName = comp.ComponentName.ToLower();

                // Check Aurora 4X Military Component Rules
                if (typeName.Contains("engine") && !compName.Contains("commercial"))
                {
                    isMilitaryComponentFound = true;
                }
                else if (typeName.Contains("beam") || typeName.Contains("weapon") || typeName.Contains("laser") || 
                         typeName.Contains("railgun") || typeName.Contains("meson") || typeName.Contains("plasma") ||
                         typeName.Contains("fire control"))
                {
                    isMilitaryComponentFound = true;
                }
                else if (typeName.Contains("launcher") || typeName.Contains("magazine") || comp.MissileCapacity > 0)
                {
                    isMilitaryComponentFound = true;
                }
                else if (typeName.Contains("active") || (typeName.Contains("sensor") && !compName.Contains("commercial")))
                {
                    isMilitaryComponentFound = true;
                }
                else if (typeName.Contains("shield") || typeName.Contains("jump") || comp.ShieldStrength > 0 || comp.HangarCapacity > 0)
                {
                    isMilitaryComponentFound = true;
                }

                // Sum Specific Attributes
                enginePower += comp.EnginePower * item.Quantity;
                fuelCap += comp.FuelCapacity * item.Quantity;
                msp += comp.MaintSupplies * item.Quantity;
                shieldStr += comp.ShieldStrength * item.Quantity;
                hangarCap += comp.HangarCapacity * item.Quantity;
                magCap += comp.MissileCapacity * item.Quantity;

                if (typeName.Contains("maintenance") || compName.Contains("engineering"))
                {
                    engineeringHS += item.TotalHS;
                }

                if (typeName.Contains("jump"))
                {
                    hasJumpDrive = true;
                    jumpSquadron = Math.Max(jumpSquadron, (int)comp.JumpRating);
                    jumpMaxHS = Math.Max(jumpMaxHS, comp.JumpMaxHS);
                }

                if (compName.Contains("crew quarters") || typeName.Contains("habitation"))
                {
                    crewQuartersProvided += (int)(item.TotalHS * 50);
                }

                // Sum Minerals
                foreach (var kvp in comp.MineralCosts)
                {
                    minerals.Add(kvp.Key, kvp.Value * item.Quantity);
                }
            }

            // Automatic Military Status determination
            design.IsMilitary = isMilitaryComponentFound || (design.ArmorThickness > 2);

            // Armor Calculations
            double armorHS = (design.ArmorThickness * design.ArmorWidth) / 50.0;
            double armorCost = design.ArmorThickness * design.ArmorWidth * 0.5;
            minerals.Add("Neutronium", armorCost * 0.7);
            minerals.Add("Duranium", armorCost * 0.3);

            design.TotalHS = compHS + armorHS;
            design.TotalCostBP = compCost + armorCost;
            design.TotalEnginePower = enginePower;
            design.TotalFuelLiters = fuelCap;
            design.TotalMSP = msp;
            design.EngineeringSpacesHS = engineeringHS;
            design.ShieldStrength = shieldStr;
            design.HangarCapacityTons = hangarCap * 50.0;
            design.MagazineCapacity = magCap;
            design.HasJumpDrive = hasJumpDrive;
            design.MaxJumpSquadron = jumpSquadron;
            design.TotalCrewRequired = crewNeeded;
            design.CrewQuartersProvidedHS = crewQuartersProvided;
            design.Minerals = minerals;

            // Computed Performance
            design.MaxSpeedKmS = FormulaCalculator.CalculateSpeed(enginePower, design.TotalHS);
            design.ThermalSignature = enginePower;
            design.EMSignature = shieldStr;

            // Fuel Consumption (Approx 0.1L per EP per hour)
            double fuelConsPerHour = enginePower * 0.1;
            design.FuelConsumptionLitersPerHour = fuelConsPerHour;
            design.RangeBillionKm = FormulaCalculator.CalculateRangeBillionKm(design.MaxSpeedKmS, fuelCap, fuelConsPerHour);
            design.FlightDaysAtFullSpeed = fuelConsPerHour > 0 ? (fuelCap / fuelConsPerHour) / 24.0 : 0;

            // Reliability
            design.AnnualFailureRate = FormulaCalculator.CalculateAnnualFailureRate(design.TotalHS, engineeringHS);
            design.MTBFMonths = design.AnnualFailureRate > 0 ? Math.Round(12.0 / design.AnnualFailureRate, 1) : 999;
            design.MaintenanceLifeYears = FormulaCalculator.CalculateMaintenanceLife(msp, design.AnnualFailureRate, design.TotalCostBP);

            // Rules & Warnings
            if (enginePower == 0)
            {
                design.Warnings.Add("⚠️ Propulsión ausente: La nave no tiene motores instalados (Velocidad: 0 km/s).");
            }
            if (crewQuartersProvided < crewNeeded && design.PlannedDeploymentMonths > 1)
            {
                design.Warnings.Add($"⚠️ Alojamiento insuficiente: Se requieren {crewNeeded} plazas de tripulación pero solo hay espacio para {crewQuartersProvided}. La moral decaerá rápidamente.");
            }
            if (hasJumpDrive && jumpMaxHS < design.TotalHS)
            {
                design.Warnings.Add($"⚠️ Motor de Salto subdimensionado: Capacidad máxima del motor de salto ({jumpMaxHS} HS) es menor que el tamaño total de la nave ({Math.Ceiling(design.TotalHS)} HS).");
            }
            if (fuelCap == 0 && enginePower > 0)
            {
                design.Warnings.Add("⚠️ Sin Combustible: La nave tiene motores pero 0 Litros de tanque de combustible.");
            }
            if (engineeringHS == 0 && design.IsMilitary && design.TotalHS > 2)
            {
                design.Suggestions.Add("💡 Sugerencia: Añadir Espacios de Ingeniería para reducir la tasa de fallos mecánicos.");
            }

            return design;
        }
    }
}
