using DragnetControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class ScriptChecker
{
    private string username;
    private bool checkCurator;
    private bool checkAggregator;
    private bool checkQuickCurator;
    private bool checkSlowCurator;
    private bool checkArbitrageEngine;
    private bool checkCoinbaseScanner;
    private bool checkKrakenScanner;
    private bool checkBinanceScanner;
    private bool checkCryptoScanner;
    private bool checkArbitrageScreener;

    private CancellationTokenSource cancellationTokenSource;

    public ScriptChecker(string username, bool checkCurator, bool checkAggregator, bool checkQuickCurator, bool checkSlowCurator, bool checkArbitrageEngine, bool checkCoinbaseScanner, bool checkKrakenScanner, bool checkBinanceScanner, bool checkCryptoScanner, bool checkArbitrageScreener)
    {
        this.username = username;
        this.checkCurator = checkCurator;
        this.checkAggregator = checkAggregator;
        this.checkQuickCurator = checkQuickCurator;
        this.checkSlowCurator = checkSlowCurator;
        this.checkArbitrageEngine = checkArbitrageEngine;
        this.checkCoinbaseScanner = checkCoinbaseScanner;
        this.checkKrakenScanner = checkKrakenScanner;
        this.checkBinanceScanner = checkBinanceScanner;
        this.checkCryptoScanner = checkCryptoScanner;
        this.checkArbitrageScreener = checkArbitrageScreener;
    }

    public void StartCheckingScripts()
    {
        cancellationTokenSource = new CancellationTokenSource();
        Task.Run(() => CheckScripts(cancellationTokenSource.Token));
    }
    public void StopCheckingScripts()
    {
        // Trigger cancellation to stop the loop in CheckScripts
        cancellationTokenSource?.Cancel();
    }
    private void CheckScripts(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Script checking has been cancelled.");
                return;
            }
            if (checkCurator && !IsProcessOpen("Curator"))
            {
                StartProcess("Curator.exe", GlobalVariables.CuratorPath, username);
            }
            if (checkArbitrageEngine && !IsProcessOpen("ArbitrageEngine"))
            {
                StartProcess("ArbitrageEngine.exe", GlobalVariables.CoinbaseScannerPath, username);
            }
            if (checkCoinbaseScanner && !IsProcessOpen("CoinbaseScanner"))
            {
                StartProcess("CoinbaseScanner.exe", GlobalVariables.CoinbaseScannerPath, username);
            }
            if (checkKrakenScanner && !IsProcessOpen("KrakenScanner"))
            {
                StartProcess("KrakenScanner.exe", GlobalVariables.KrakenPath, username);
            }
            if (checkBinanceScanner && !IsProcessOpen("BinanceScanner"))
            {
                StartProcess("BinanceScanner.exe", GlobalVariables.BinancePath, username);
            }
            if (checkCryptoScanner && !IsProcessOpen("CryptoScanner"))
            {
                StartProcess("CryptoScanner.exe", GlobalVariables.KrakenPath, username);
            }

            Thread.Sleep(5000);  // Sleep for 5 seconds before checking again
        }
    }

    private bool IsProcessOpen(string name)
    {
        foreach (Process clsProcess in Process.GetProcesses())
        {
            if (clsProcess.ProcessName.Contains(name))
            {
                return true;
            }
        }
        return false;
    }

    private void StartProcess(string fileName, string workingDirectory, string arguments, bool createNoWindow = false)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            Arguments = $"\"{arguments}\"",
            CreateNoWindow = createNoWindow,
            UseShellExecute = true,
        };
        Process.Start(startInfo);
    }
}


