using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class XenoIntelView : UserControl
    {
        private DatabaseService? _dbService;
        private int _raceId;
        private List<AlienRaceInfo> _allAlienRaces = new List<AlienRaceInfo>();

        public XenoIntelView()
        {
            InitializeComponent();
        }

        public void LoadData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _raceId = raceId;
            RefreshData();
        }

        public void RefreshData()
        {
            if (_dbService == null || _raceId <= 0) return;

            _allAlienRaces = _dbService.GetAlienRaces(_raceId);
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string query = TxtSearchXeno?.Text?.Trim().ToLower() ?? "";
            var filtered = _allAlienRaces.Where(a =>
                string.IsNullOrEmpty(query) ||
                a.AlienRaceName.ToLower().Contains(query) ||
                a.Abbrev.ToLower().Contains(query) ||
                a.CommStatusDisplay.ToLower().Contains(query)
            ).ToList();

            DgAlienRaces.ItemsSource = filtered;

            if (TxtXenoSummary != null)
            {
                TxtXenoSummary.Text = filtered.Count > 0
                    ? $"{filtered.Count} Especies Alienígenas Contactadas"
                    : "🛡️ Alerta Preventiva: 0 Contactos Alienígenas Registrados";
            }

            if (filtered.Count > 0 && DgAlienRaces.SelectedItem == null)
            {
                DgAlienRaces.SelectedIndex = 0;
            }
            else if (filtered.Count == 0)
            {
                // Standby mode when no NPR contacts exist yet
                if (TxtXenoHeader != null) TxtXenoHeader.Text = "🛡️ PROTOCOLO IMPERIAL DE PRIMER CONTACTO (STANDBY)";
                if (LblXenoPoints != null) LblXenoPoints.Text = "0,0 pts";
                if (LblXenoTreaties != null) LblXenoTreaties.Text = "Sin Contactos";
                if (LblXenoDamage != null) LblXenoDamage.Text = "0 t (Flota Intacta)";
                if (TxtProtocolText != null)
                {
                    TxtProtocolText.Text = "🛰️ Red de Sensores Pasivos Térmicos y Gravimétricos activa en todos los Puntos de Salto imperiales. No se han avistado naves de civilizaciones alienígenas (NPR). La Flota se mantiene en Estado de Preparación Alfa.";
                }
                DgAlienClasses.ItemsSource = null;
            }
        }

        private void DgAlienRaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgAlienRaces.SelectedItem is not AlienRaceInfo selected) return;

            if (TxtXenoHeader != null) TxtXenoHeader.Text = $"👽 {selected.AlienRaceName} ({selected.Abbrev}) - ANÁLISIS TÁCTICO";
            if (LblXenoPoints != null) LblXenoPoints.Text = $"{selected.DiplomaticPoints:N1} Puntos";
            if (LblXenoTreaties != null) LblXenoTreaties.Text = selected.TreatiesSummary;
            if (LblXenoDamage != null) LblXenoDamage.Text = $"{selected.DamageCaused:N0} t Causadas";

            DgAlienClasses.ItemsSource = selected.Classes;

            if (TxtProtocolText != null)
            {
                if (selected.DamageCaused > 0)
                {
                    TxtProtocolText.Text = $"🔴 ALERTA ROJA IMPERIAL: Especie hostil confirmada ({selected.DamageCaused:N0} t de daño provocado). Se autoriza el uso de fuerza letal y despliegue de escuadrones de interceptación.";
                }
                else if (selected.DiplomaticPoints < 0)
                {
                    TxtProtocolText.Text = $"🟠 ALERTA AMARILLA: Relaciones en fricción diplomática. Monitorear movimientos de patrullas alienígenas en sectores fronterizos.";
                }
                else
                {
                    TxtProtocolText.Text = $"🟢 ALERTA VERDE: Xenodiplomacia pacífica. Las vías de negociación e intercambio de datos están operativas.";
                }
            }
        }

        private void TxtSearchXeno_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void BtnRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshData();
        }
    }
}
