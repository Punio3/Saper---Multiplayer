using System;
using System.Diagnostics;
using System.Timers;

namespace multigame.Game.GameLogic
{

    public class GameTimer
    {
        private Stopwatch _stopwatch;
        private System.Timers.Timer _updateTimer; // Precyzujesz, której klasy Timer używasz

        public event EventHandler<string> TimeUpdated;

        public GameTimer()
        {
            _stopwatch = new Stopwatch();
            _updateTimer = new System.Timers.Timer(1000); // Tutaj również jasno wskazujesz Timer z System.Timers
            _updateTimer.Elapsed += OnTimerElapsed;
        }

        // Metoda uruchamiania stopera
        public void StartTimer()
        {
            _stopwatch.Start();
            _updateTimer.Start(); // Timer będzie uruchomiony i będzie aktualizował czas co sekundę
        }

        // Metoda zatrzymania stopera
        public void StopTimer()
        {
            _stopwatch.Stop();
            _updateTimer.Stop(); // Zatrzymuje odliczanie
        }

        // Metoda resetująca czas
        public void ResetTimer()
        {
            _stopwatch.Reset();
            TimeUpdated?.Invoke(this, FormatTime(_stopwatch.Elapsed)); // Zaktualizowanie wyświetlanego czasu po resecie
        }

        // Metoda formatowania czasu w bardziej czytelny sposób (godziny:minuty:sekundy)
        private string FormatTime(TimeSpan time)
        {
            return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
        }

        // Metoda wywoływana przy każdym tyknięciu zegara (każda sekunda)
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimeUpdated?.Invoke(this, FormatTime(_stopwatch.Elapsed)); // Wydarzenie informujące o aktualizacji czasu
        }
    }
}
