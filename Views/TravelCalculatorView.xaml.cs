using System;
using System.Windows;
using System.Windows.Controls;

namespace AuroraDesignSuite.Views
{
    public partial class TravelCalculatorView : UserControl
    {
        public TravelCalculatorView()
        {
            InitializeComponent();
            Loaded += (s, e) => OnFormulaInputChanged(null, null);
        }

        private void BtnToggleCalculators_Click(object sender, RoutedEventArgs e)
        {
            if (PnlCalculatorsContainer == null || BtnToggleCalculators == null) return;
            if (PnlCalculatorsContainer.Visibility == Visibility.Visible)
            {
                PnlCalculatorsContainer.Visibility = Visibility.Collapsed;
                BtnToggleCalculators.Content = "👁️ Mostrar Calculadoras";
            }
            else
            {
                PnlCalculatorsContainer.Visibility = Visibility.Visible;
                BtnToggleCalculators.Content = "👁️ Ocultar Calculadoras";
            }
        }

        private void BtnSelectCalculators_Click(object sender, RoutedEventArgs e)
        {
            if (PnlCalculatorSelector == null) return;
            PnlCalculatorSelector.Visibility = PnlCalculatorSelector.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void OnCalcVisibilityChanged(object sender, RoutedEventArgs e)
        {
            if (CardCalc1 == null || CardCalc2 == null || CardCalc3 == null || CardCalc4 == null || CardCalc5 == null ||
                CardCalc6 == null || CardCalc7 == null || CardCalc8 == null || CardCalc9 == null || CardCalc10 == null ||
                CardCalc11 == null || CardCalc12 == null || CardCalc13 == null || CardCalc14 == null || CardCalc15 == null ||
                CardCalc16 == null || CardCalc17 == null || CardCalc18 == null || CardCalc19 == null || CardCalc20 == null) return;

            int activeCount = 0;

            if (ChkCalc1 != null) { CardCalc1.Visibility = ChkCalc1.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc1.IsChecked == true) activeCount++; }
            if (ChkCalc2 != null) { CardCalc2.Visibility = ChkCalc2.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc2.IsChecked == true) activeCount++; }
            if (ChkCalc3 != null) { CardCalc3.Visibility = ChkCalc3.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc3.IsChecked == true) activeCount++; }
            if (ChkCalc4 != null) { CardCalc4.Visibility = ChkCalc4.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc4.IsChecked == true) activeCount++; }
            if (ChkCalc5 != null) { CardCalc5.Visibility = ChkCalc5.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc5.IsChecked == true) activeCount++; }
            if (ChkCalc6 != null) { CardCalc6.Visibility = ChkCalc6.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc6.IsChecked == true) activeCount++; }
            if (ChkCalc7 != null) { CardCalc7.Visibility = ChkCalc7.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc7.IsChecked == true) activeCount++; }
            if (ChkCalc8 != null) { CardCalc8.Visibility = ChkCalc8.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc8.IsChecked == true) activeCount++; }
            if (ChkCalc9 != null) { CardCalc9.Visibility = ChkCalc9.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc9.IsChecked == true) activeCount++; }
            if (ChkCalc10 != null) { CardCalc10.Visibility = ChkCalc10.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc10.IsChecked == true) activeCount++; }
            if (ChkCalc11 != null) { CardCalc11.Visibility = ChkCalc11.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc11.IsChecked == true) activeCount++; }
            if (ChkCalc12 != null) { CardCalc12.Visibility = ChkCalc12.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc12.IsChecked == true) activeCount++; }
            if (ChkCalc13 != null) { CardCalc13.Visibility = ChkCalc13.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc13.IsChecked == true) activeCount++; }
            if (ChkCalc14 != null) { CardCalc14.Visibility = ChkCalc14.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc14.IsChecked == true) activeCount++; }
            if (ChkCalc15 != null) { CardCalc15.Visibility = ChkCalc15.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc15.IsChecked == true) activeCount++; }
            if (ChkCalc16 != null) { CardCalc16.Visibility = ChkCalc16.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc16.IsChecked == true) activeCount++; }
            if (ChkCalc17 != null) { CardCalc17.Visibility = ChkCalc17.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc17.IsChecked == true) activeCount++; }
            if (ChkCalc18 != null) { CardCalc18.Visibility = ChkCalc18.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc18.IsChecked == true) activeCount++; }
            if (ChkCalc19 != null) { CardCalc19.Visibility = ChkCalc19.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc19.IsChecked == true) activeCount++; }
            if (ChkCalc20 != null) { CardCalc20.Visibility = ChkCalc20.IsChecked == true ? Visibility.Visible : Visibility.Collapsed; if (ChkCalc20.IsChecked == true) activeCount++; }

            if (BtnSelectCalculators != null)
            {
                BtnSelectCalculators.Content = $"🎛️ Selector de Calculadoras ({activeCount}/20) 🔻";
            }
        }

        private void BtnCheckAll_Click(object sender, RoutedEventArgs e) => SetAllCheckState(true);
        private void BtnUncheckAll_Click(object sender, RoutedEventArgs e) => SetAllCheckState(false);
        private void BtnResetDefault_Click(object sender, RoutedEventArgs e) => SetAllCheckState(true);

        private void SetAllCheckState(bool isChecked)
        {
            if (ChkCalc1 != null) ChkCalc1.IsChecked = isChecked;
            if (ChkCalc2 != null) ChkCalc2.IsChecked = isChecked;
            if (ChkCalc3 != null) ChkCalc3.IsChecked = isChecked;
            if (ChkCalc4 != null) ChkCalc4.IsChecked = isChecked;
            if (ChkCalc5 != null) ChkCalc5.IsChecked = isChecked;
            if (ChkCalc6 != null) ChkCalc6.IsChecked = isChecked;
            if (ChkCalc7 != null) ChkCalc7.IsChecked = isChecked;
            if (ChkCalc8 != null) ChkCalc8.IsChecked = isChecked;
            if (ChkCalc9 != null) ChkCalc9.IsChecked = isChecked;
            if (ChkCalc10 != null) ChkCalc10.IsChecked = isChecked;
            if (ChkCalc11 != null) ChkCalc11.IsChecked = isChecked;
            if (ChkCalc12 != null) ChkCalc12.IsChecked = isChecked;
            if (ChkCalc13 != null) ChkCalc13.IsChecked = isChecked;
            if (ChkCalc14 != null) ChkCalc14.IsChecked = isChecked;
            if (ChkCalc15 != null) ChkCalc15.IsChecked = isChecked;
            if (ChkCalc16 != null) ChkCalc16.IsChecked = isChecked;
            if (ChkCalc17 != null) ChkCalc17.IsChecked = isChecked;
            if (ChkCalc18 != null) ChkCalc18.IsChecked = isChecked;
            if (ChkCalc19 != null) ChkCalc19.IsChecked = isChecked;
            if (ChkCalc20 != null) ChkCalc20.IsChecked = isChecked;

            OnCalcVisibilityChanged(this, new RoutedEventArgs());
        }

        private void OnFormulaInputChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // 1. Missile Penetration vs Enemy Armor
                if (TxtFormulaWarheadYield != null && TxtFormulaEnemyArmor != null && LblFormulaWarheadResult != null &&
                    double.TryParse(TxtFormulaWarheadYield.Text, out double yieldVal) &&
                    double.TryParse(TxtFormulaEnemyArmor.Text, out double armorLayers) && yieldVal > 0)
                {
                    int penetration = (int)Math.Sqrt(yieldVal);
                    if (penetration < 1) penetration = 1;
                    int remaining = (int)armorLayers - penetration;
                    if (remaining <= 0)
                    {
                        LblFormulaWarheadResult.Text = $"✅ ¡PERFORACIÓN COMPLETA! Cráter: {penetration} Prof x {penetration} Ancho | Perfora las {armorLayers} capas de blindaje enemigas causando daño interno directo a sistemas.";
                    }
                    else
                    {
                        LblFormulaWarheadResult.Text = $"⚠️ ALERTA: EL BLINDAJE SOPORTA EL IMPACTO DIRECTO. Cráter: {penetration} Prof x {penetration} Ancho | El blindaje enemigo absorbe el golpe (quedan {remaining} capas intactas). Se requieren ojivas de Yield {Math.Pow(armorLayers, 2):N0}+ para penetración directa.";
                    }
                }

                // 2. Active Sensor Range vs Target
                if (TxtFormulaRadarStrength != null && TxtFormulaRadarRes != null && TxtFormulaTargetHS != null && LblFormulaRadarResult != null &&
                    double.TryParse(TxtFormulaRadarStrength.Text, out double strength) &&
                    double.TryParse(TxtFormulaRadarRes.Text, out double res) &&
                    double.TryParse(TxtFormulaTargetHS.Text, out double targetHS) && strength > 0 && res > 0 && targetHS > 0)
                {
                    double baseMaxKm = strength * Math.Sqrt(res) * 400000.0;
                    double effRangeKm = baseMaxKm * Math.Min(1.0, Math.Sqrt(targetHS / res));
                    double effRangeMkm = effRangeKm / 1_000_000.0;

                    if (targetHS >= res)
                    {
                        LblFormulaRadarResult.Text = $"✅ Detección a Alcance Máximo del Radar: {effRangeMkm:N2} Mkm ({effRangeKm:N0} km). El blanco de {targetHS} HS ({targetHS*50} tons) supera la resolución del radar (Res {res}).";
                    }
                    else
                    {
                        LblFormulaRadarResult.Text = $"⚠️ Alcance Reducido por Firma Pequeña: {effRangeMkm:N2} Mkm ({effRangeKm:N0} km). El objetivo ({targetHS} HS) es más pequeño que la resolución del radar (Res {res}).";
                    }
                }

                // 3. Fleet Engine Burn & Tanker Logistics
                if (TxtFormulaEngineEP != null && TxtFormulaEngineFuelRate != null && TxtFormulaFleetShips != null && TxtFormulaMissionDays != null && LblFormulaEngineResult != null &&
                    double.TryParse(TxtFormulaEngineEP.Text, out double ep) &&
                    double.TryParse(TxtFormulaEngineFuelRate.Text, out double rate) &&
                    double.TryParse(TxtFormulaFleetShips.Text, out double shipsCount) &&
                    double.TryParse(TxtFormulaMissionDays.Text, out double missionDays) && ep > 0 && rate > 0 && shipsCount > 0 && missionDays > 0)
                {
                    double lphPerShip = ep * rate;
                    double lpdPerShip = lphPerShip * 24.0;
                    double totalFleetFuel = lpdPerShip * shipsCount * missionDays;
                    double tankersReq = Math.Ceiling(totalFleetFuel / 1_000_000.0);

                    LblFormulaEngineResult.Text = $"⛽ Consumo Flota: {totalFleetFuel:N0} Litros totales en {missionDays} días ({lpdPerShip:N0} L/día por nave x {shipsCount} naves). Logística requerida: {tankersReq:N0} Naves Cisterna de 1M L.";
                }

                // 4. Retooling Cost & Duration
                if (TxtFormulaRetoolCapacity != null && TxtFormulaRetoolDiff != null && TxtFormulaRetoolBP != null && LblFormulaRetoolResult != null &&
                    double.TryParse(TxtFormulaRetoolCapacity.Text, out double capTons) &&
                    double.TryParse(TxtFormulaRetoolDiff.Text, out double diffPct) &&
                    double.TryParse(TxtFormulaRetoolBP.Text, out double bpYear) && capTons > 0 && bpYear > 0)
                {
                    double normDiff = Math.Min(100.0, Math.Max(0.0, diffPct)) / 100.0;
                    double retoolCostBP = capTons * 0.05 * (0.20 + 0.80 * normDiff);
                    double retoolDays = Math.Round(retoolCostBP / (bpYear / 365.0), 0);

                    LblFormulaRetoolResult.Text = $"🏗️ Re-equipamiento: {retoolCostBP:N0} BP requeridos | Tiempo estimado: {retoolDays:N0} Días ({retoolDays/30.0:F1} meses a {bpYear} BP/año por grada).";
                }

                // 5. Naval Speed & Interception
                if (TxtFormulaSpeedEP != null && TxtFormulaSpeedHS != null && TxtFormulaEnemySpeed != null && LblFormulaSpeedResult != null &&
                    double.TryParse(TxtFormulaSpeedEP.Text, out double speedEP) &&
                    double.TryParse(TxtFormulaSpeedHS.Text, out double speedHS) &&
                    double.TryParse(TxtFormulaEnemySpeed.Text, out double enemySpeed) && speedHS > 0)
                {
                    double speedKmS = (speedEP / speedHS) * 1000.0;
                    double speedDiff = speedKmS - enemySpeed;

                    if (speedDiff >= 0)
                    {
                        LblFormulaSpeedResult.Text = $"✅ Ventaja Táctica de Velocidad: {speedKmS:N0} km/s vs {enemySpeed:N0} km/s enemigo (+{speedDiff:N0} km/s de superioridad). Capacidad de dar alcance o romper el contacto a voluntad.";
                    }
                    else
                    {
                        LblFormulaSpeedResult.Text = $"❌ DEFICIT DE VELOCIDAD: {speedKmS:N0} km/s vs {enemySpeed:N0} km/s enemigo ({speedDiff:N0} km/s más lento). El enemigo mantendrá la iniciativa táctica de distancia en combate.";
                    }
                }

                // 6. Flight Endurance & Safe Return Radius
                if (TxtFormulaRangeFuel != null && TxtFormulaRangeRate != null && TxtFormulaRangeSpeed != null && LblFormulaRangeResult != null &&
                    double.TryParse(TxtFormulaRangeFuel.Text, out double totalFuel) &&
                    double.TryParse(TxtFormulaRangeRate.Text, out double lph) &&
                    double.TryParse(TxtFormulaRangeSpeed.Text, out double cruiseSpeed) && totalFuel > 0 && lph > 0 && cruiseSpeed > 0)
                {
                    double flightHours = totalFuel / lph;
                    double flightDays = flightHours / 24.0;
                    double totalDistBkm = (flightHours * cruiseSpeed * 3600.0) / 1_000_000_000.0;
                    double safeReturnBkm = totalDistBkm * 0.5;

                    LblFormulaRangeResult.Text = $"🌌 Autonomía Total: {totalDistBkm:N2} Bkm ({totalDistBkm/0.14959787:N2} AU) | Radio de Retorno Seguro (50% Combustible): {safeReturnBkm:N2} Bkm ({flightDays/2.0:F1} días ida).";
                }

                // 7. Maintenance AFR & Autonomous Months
                if (TxtFormulaMaintBP != null && TxtFormulaMaintDCR != null && TxtFormulaMaintMSP != null && LblFormulaMaintResult != null &&
                    double.TryParse(TxtFormulaMaintBP.Text, out double shipCostBP) &&
                    double.TryParse(TxtFormulaMaintDCR.Text, out double dcrRating) &&
                    double.TryParse(TxtFormulaMaintMSP.Text, out double mspStock) && shipCostBP > 0)
                {
                    double effDCR = Math.Max(1.0, dcrRating);
                    double afrPercent = Math.Min(100.0, Math.Max(1.0, (shipCostBP / (effDCR * 5.0))));
                    double mtbfDays = Math.Round(365.0 / (afrPercent / 100.0), 0);
                    double mspPerYear = shipCostBP * (afrPercent / 100.0) * 0.1;
                    double monthsSupply = mspPerYear > 0 ? (mspStock / mspPerYear) * 12.0 : 99.0;

                    LblFormulaMaintResult.Text = $"🔧 Tasa AFR: {afrPercent:F1}% | MTBF: ~{mtbfDays:N0} Días entre averías | Autonomía de Pañol MSP ({mspStock} unidades): {monthsSupply:F1} Meses en espacio profundo.";
                }

                // 8. Planet Colony Cost & Cargo Freighters
                if (TxtFormulaColonyTemp != null && TxtFormulaColonyO2 != null && TxtFormulaColonyTox != null && TxtFormulaColonyPop != null && LblFormulaColonyResult != null &&
                    double.TryParse(TxtFormulaColonyTemp.Text, out double colTemp) &&
                    double.TryParse(TxtFormulaColonyO2.Text, out double colO2) &&
                    double.TryParse(TxtFormulaColonyTox.Text, out double colTox) &&
                    double.TryParse(TxtFormulaColonyPop.Text, out double colPopM))
                {
                    double tempDiff = Math.Abs(colTemp - 15.0);
                    double tempCost = tempDiff > 30.0 ? (tempDiff - 30.0) / 20.0 : 0.0;
                    double o2Cost = colO2 < 0.10 ? 2.0 : (colO2 > 0.30 ? 3.0 : 0.0);
                    double toxCost = colTox > 0.0 ? colTox * 4.0 : 0.0;
                    double netCost = Math.Round(tempCost + o2Cost + toxCost, 2);
                    double totalInfra = netCost * 100.0 * colPopM;
                    double freightersReq = Math.Ceiling(totalInfra / 25.0);

                    LblFormulaColonyResult.Text = $"🌍 Colony Cost: {netCost:F2} | Infraestructura Necesaria: {totalInfra:N0} unidades para {colPopM}M habs ({freightersReq:N0} fletes de cargueros de 25k tons).";
                }

                // 9. Mining Output & Ore Reserves
                if (TxtFormulaMiningCount != null && TxtFormulaMiningAcc != null && TxtFormulaMiningReserves != null && LblFormulaMiningResult != null &&
                    double.TryParse(TxtFormulaMiningCount.Text, out double mineCount) &&
                    double.TryParse(TxtFormulaMiningAcc.Text, out double mineAcc) &&
                    double.TryParse(TxtFormulaMiningReserves.Text, out double reservesTons) && mineCount > 0 && mineAcc > 0)
                {
                    double outputPerMin = mineCount * 12.0 * Math.Min(1.0, mineAcc);
                    double total11Mins = outputPerMin * 11.0;
                    double yearsExhaustion = reservesTons > 0 ? reservesTons / outputPerMin : 0.0;

                    LblFormulaMiningResult.Text = $"⛏️ Extracción: {outputPerMin:N0} Tons/Año por mineral ({total11Mins:N0} Tons totales 11 mins). Agotamiento del yacimiento ({reservesTons:N0} tons): ~{yearsExhaustion:F1} Años.";
                }

                // 10. Thermal Stealth & Infiltration
                if (TxtFormulaStealthTH != null && TxtFormulaStealthCover != null && TxtFormulaStealthSens != null && LblFormulaStealthResult != null &&
                    double.TryParse(TxtFormulaStealthTH.Text, out double thermalSig) &&
                    double.TryParse(TxtFormulaStealthCover.Text, out double stealthPct) &&
                    double.TryParse(TxtFormulaStealthSens.Text, out double sensPower) && thermalSig > 0 && sensPower > 0)
                {
                    double netSig = thermalSig * (1.0 - (Math.Min(90.0, stealthPct) / 100.0));
                    double detRangeKm = netSig * sensPower * 10000.0;
                    double detRangeMkm = detRangeKm / 1_000_000.0;

                    LblFormulaStealthResult.Text = $"🛰️ Firma Térmica Neta: {netSig:F0} TH (Reducción {stealthPct}%) | Distancia Detección Enemiga: {detRangeMkm:N2} Mkm ({detRangeKm:N0} km).";
                }

                // 11. Shield Absorptive Power & Recharge
                if (TxtFormulaShieldCount != null && TxtFormulaShieldTech != null && TxtFormulaShieldRecharge != null && LblFormulaShieldResult != null &&
                    double.TryParse(TxtFormulaShieldCount.Text, out double shCount) &&
                    double.TryParse(TxtFormulaShieldTech.Text, out double shPower) &&
                    double.TryParse(TxtFormulaShieldRecharge.Text, out double shRate) && shCount > 0 && shPower > 0 && shRate > 0)
                {
                    double totalShields = shCount * shPower;
                    double secsRecharge = totalShields / shRate;
                    double regenPerTurn = shRate * 5.0;

                    LblFormulaShieldResult.Text = $"🛡️ Escudos Totales: {totalShields:N0} Puntos de Absorción | Tiempo Recarga 100%: {secsRecharge:N0} seg ({secsRecharge/60.0:F1} min) | Regeneración por Turno 5s: +{regenPerTurn:F1} Pts.";
                }

                // 12. Beam Capacitor Recharge & Power Balance
                if (TxtFormulaBeamPower != null && TxtFormulaBeamCount != null && TxtFormulaCapacitorRate != null && TxtFormulaReactorPower != null && LblFormulaBeamResult != null &&
                    double.TryParse(TxtFormulaBeamPower.Text, out double beamEU) &&
                    double.TryParse(TxtFormulaBeamCount.Text, out double beamCount) &&
                    double.TryParse(TxtFormulaCapacitorRate.Text, out double capRate) &&
                    double.TryParse(TxtFormulaReactorPower.Text, out double reactorEU) && beamEU > 0 && beamCount > 0 && capRate > 0)
                {
                    double totalEUNeeded = beamEU * beamCount;
                    double turnsToCharge = Math.Ceiling(beamEU / capRate);
                    double secsToFire = turnsToCharge * 5.0;
                    double totalEUSupplyPerTurn = reactorEU;
                    double totalEUDemandPerTurn = capRate * beamCount;

                    if (totalEUSupplyPerTurn >= totalEUDemandPerTurn)
                    {
                        LblFormulaBeamResult.Text = $"⚡ Disparo Láser: 1 Cadencia cada {secsToFire:N0} seg | Demand: {totalEUDemandPerTurn} EU/5s vs Reactores: {totalEUSupplyPerTurn} EU/5s (✅ Reactores Suficientes).";
                    }
                    else
                    {
                        LblFormulaBeamResult.Text = $"⚡ Disparo Láser: 1 Cadencia cada {secsToFire:N0} seg | Demand: {totalEUDemandPerTurn} EU/5s vs Reactores: {totalEUSupplyPerTurn} EU/5s (❌ DÉFICIT DE ENERGÍA - Añadir reactores).";
                    }
                }

                // 13. Missile Hit Probability vs ECM/ECCM
                if (TxtFormulaMissileSpeed != null && TxtFormulaTargetSpeed != null && TxtFormulaTargetECM != null && TxtFormulaMissileECCM != null && LblFormulaMissileHitResult != null &&
                    double.TryParse(TxtFormulaMissileSpeed.Text, out double mSpeed) &&
                    double.TryParse(TxtFormulaTargetSpeed.Text, out double tSpeed) &&
                    double.TryParse(TxtFormulaTargetECM.Text, out double ecmLvl) &&
                    double.TryParse(TxtFormulaMissileECCM.Text, out double eccmLvl) && mSpeed > 0 && tSpeed > 0)
                {
                    double speedRatio = mSpeed / tSpeed;
                    double baseHit = Math.Min(100.0, speedRatio * 37.5);
                    double netECM = Math.Max(0.0, ecmLvl - eccmLvl);
                    double ecmPen = netECM * 10.0;
                    double finalHit = Math.Max(1.0, baseHit - ecmPen);

                    LblFormulaMissileHitResult.Text = $"🎯 Probabilidad de Impacto: {finalHit:F1}% (Base Velocidad: {baseHit:F1}% | Penalización Net ECM -{ecmPen}% [Jammer {ecmLvl} vs ECCM {eccmLvl}]).";
                }

                // 14. Industrial Construction Capacity & Project Days
                if (TxtFormulaFactoriesCount != null && TxtFormulaGovMod != null && TxtFormulaProjectBP != null && LblFormulaConstResult != null &&
                    double.TryParse(TxtFormulaFactoriesCount.Text, out double facCount) &&
                    double.TryParse(TxtFormulaGovMod.Text, out double govMod) &&
                    double.TryParse(TxtFormulaProjectBP.Text, out double projBP) && facCount > 0 && projBP > 0)
                {
                    double totalBPYear = facCount * 10.0 * (1.0 + (govMod / 100.0));
                    double totalBPDay = totalBPYear / 365.0;
                    double daysToComplete = Math.Round(projBP / totalBPDay, 0);

                    LblFormulaConstResult.Text = $"🏭 Producción Industrial: {totalBPYear:N0} BP/Año ({totalBPDay:F1} BP/Día) | Proyecto de {projBP:N0} BP completado en ~{daysToComplete:N0} Días ({daysToComplete/30.0:F1} meses).";
                }

                // 15. EM Passive Sensor Detection
                if (TxtFormulaEMSensingSig != null && TxtFormulaEMSensRating != null && TxtFormulaEMSensCount != null && LblFormulaEMResult != null &&
                    double.TryParse(TxtFormulaEMSensingSig.Text, out double emSig) &&
                    double.TryParse(TxtFormulaEMSensRating.Text, out double emRating) &&
                    double.TryParse(TxtFormulaEMSensCount.Text, out double emCount) && emSig > 0 && emRating > 0 && emCount > 0)
                {
                    double emRangeKm = emSig * emRating * Math.Sqrt(emCount) * 10000.0;
                    double emRangeMkm = emRangeKm / 1_000_000.0;

                    LblFormulaEMResult.Text = $"🛰️ Alcance Pasivo EM: {emRangeMkm:N2} Mkm ({emRangeKm:N0} km). Alarma silenciosa de escudos/radares activados en naves enemigas.";
                }

                // 16. Mass Driver Transport & Collision Safety
                if (TxtFormulaMassDriverSender != null && TxtFormulaMassDriverReceiver != null && TxtFormulaDriverDist != null && LblFormulaMassDriverResult != null &&
                    double.TryParse(TxtFormulaMassDriverSender.Text, out double senders) &&
                    double.TryParse(TxtFormulaMassDriverReceiver.Text, out double receivers) &&
                    double.TryParse(TxtFormulaDriverDist.Text, out double distMkm) && senders > 0)
                {
                    double tonsYear = senders * 5000.0;
                    double transitDays = Math.Round(distMkm / 43.2, 1);

                    if (receivers >= senders)
                    {
                        LblFormulaMassDriverResult.Text = $"🚀 Transferencia Masa: {tonsYear:N0} Tons/Año (Tránsito: {transitDays:F1} días) | Status Recepción: ✅ SEGURO ({receivers} receptoras capturan los envíos).";
                    }
                    else
                    {
                        LblFormulaMassDriverResult.Text = $"🚀 Transferencia Masa: {tonsYear:N0} Tons/Año (Tránsito: {transitDays:F1} días) | Status Recepción: 💥 RIESGO IMPACTO COLISIONAL DEVASTADOR ({senders-receivers} paquetes sin capturar).";
                    }
                }

                // 17. Research RP Output & Scientist Director
                if (TxtFormulaLabsCount != null && TxtFormulaSciBonus != null && TxtFormulaTechRP != null && LblFormulaResearchResult != null &&
                    double.TryParse(TxtFormulaLabsCount.Text, out double labCount) &&
                    double.TryParse(TxtFormulaSciBonus.Text, out double sciBonus) &&
                    double.TryParse(TxtFormulaTechRP.Text, out double techRP) && labCount > 0 && techRP > 0)
                {
                    double rpYear = labCount * 200.0 * (1.0 + (sciBonus / 100.0));
                    double rpMonth = rpYear / 12.0;
                    double yearsToFinish = techRP / rpYear;
                    double monthsToFinish = yearsToFinish * 12.0;

                    LblFormulaResearchResult.Text = $"🎓 Generación I+D: {rpYear:N0} RP/Año ({rpMonth:N0} RP/Mes) | Tecnología de {techRP:N0} RP investigada en {yearsToFinish:F1} Años ({monthsToFinish:F0} Meses).";
                }

                // 18. Point Defense Interception vs Salvo
                if (TxtFormulaGaussCount != null && TxtFormulaTrackingSpeed != null && TxtFormulaEnemyMissileSpeed != null && TxtFormulaSalvoSize != null && LblFormulaPointDefResult != null &&
                    double.TryParse(TxtFormulaGaussCount.Text, out double gaussCount) &&
                    double.TryParse(TxtFormulaTrackingSpeed.Text, out double trackSpeed) &&
                    double.TryParse(TxtFormulaEnemyMissileSpeed.Text, out double mSpeedEnemy) &&
                    double.TryParse(TxtFormulaSalvoSize.Text, out double salvoSize) && gaussCount > 0 && mSpeedEnemy > 0)
                {
                    double trackRatio = Math.Min(1.0, trackSpeed / mSpeedEnemy);
                    double baseAcc = trackRatio * 50.0;
                    double totalShots = gaussCount * 3.0;
                    double intercepted = Math.Round(totalShots * (baseAcc / 100.0), 1);
                    double leaked = Math.Max(0.0, salvoSize - intercepted);

                    if (leaked == 0)
                    {
                        LblFormulaPointDefResult.Text = $"💣 Precisión CIWS: {baseAcc:F1}% | Interceptados: {intercepted:F1} de {salvoSize} misiles | Status: ✅ SALVA TOTALMENTE DESTRUIDA EN EL AIRE.";
                    }
                    else
                    {
                        LblFormulaPointDefResult.Text = $"💣 Precisión CIWS: {baseAcc:F1}% | Interceptados: {intercepted:F1} de {salvoSize} misiles | Status: ⚠️ ALERTA ({leaked:F1} misiles impactarán el casco).";
                    }
                }

                // 19. Naval Maintenance Limit & Drydock Capacity
                if (TxtFormulaMaintFacCount != null && TxtFormulaColonyPopMaint != null && TxtFormulaActiveTonnage != null && LblFormulaNavalMaintResult != null &&
                    double.TryParse(TxtFormulaMaintFacCount.Text, out double maintFacs) &&
                    double.TryParse(TxtFormulaColonyPopMaint.Text, out double colPopMaint) &&
                    double.TryParse(TxtFormulaActiveTonnage.Text, out double fleetTons) && maintFacs > 0)
                {
                    double maxFacTons = maintFacs * 2000.0;
                    double maxPopTons = colPopMaint * 500.0;
                    double maxTotalSupported = Math.Min(maxFacTons, maxPopTons);

                    if (fleetTons <= maxTotalSupported)
                    {
                        LblFormulaNavalMaintResult.Text = $"⚓ Capacidad Dique Seco: {maxTotalSupported:N0} Tons sop. | Flota Estacionada: {fleetTons:N0} Tons | Status: ✅ MANTENIMIENTO COMPLETO SIN DEGRADACIÓN.";
                    }
                    else
                    {
                        LblFormulaNavalMaintResult.Text = $"⚓ Capacidad Dique Seco: {maxTotalSupported:N0} Tons sop. | Flota Estacionada: {fleetTons:N0} Tons | Status: ❌ EXCESO DE TONELAJE ({fleetTons-maxTotalSupported:N0} tons sin soporte sufren averías).";
                    }
                }

                // 20. Empire Wealth Generation & Expenses
                if (TxtFormulaPopMillions != null && TxtFormulaFinancialCount != null && TxtFormulaMilitaryExpenses != null && LblFormulaWealthResult != null &&
                    double.TryParse(TxtFormulaPopMillions.Text, out double popM) &&
                    double.TryParse(TxtFormulaFinancialCount.Text, out double finCount) &&
                    double.TryParse(TxtFormulaMilitaryExpenses.Text, out double milExp))
                {
                    double popIncome = popM * 1000.0;
                    double finIncome = finCount * 25000.0;
                    double totalIncome = popIncome + finIncome;
                    double netBalance = totalIncome - milExp;

                    if (netBalance >= 0)
                    {
                        LblFormulaWealthResult.Text = $"💵 Ingresos: {totalIncome:N0} Riqueza/Año (Población: {popIncome:N0} + Finanzas: {finIncome:N0}) | Gastos Militares: {milExp:N0} | Balance: ✅ SUPERÁVIT +{netBalance:N0} Riqueza/Año.";
                    }
                    else
                    {
                        LblFormulaWealthResult.Text = $"💵 Ingresos: {totalIncome:N0} Riqueza/Año (Población: {popIncome:N0} + Finanzas: {finIncome:N0}) | Gastos Militares: {milExp:N0} | Balance: ❌ DÉFICIT {netBalance:N0} Riqueza/Año (Riesgo bancarrota).";
                    }
                }
            }
            catch { }
        }
    }
}
