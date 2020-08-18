/*
MIT License

Copyright (c) 2019-2020 César Andrés Morgan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

===============================================================================
V U L C A N I U M - A R I C H U A
  _.----._
 (   (    )
(  (    )  )
 (________)
    ||||
  --++++--
    ||||
  .(    ).
 (_(____)_)
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Threading.Tasks;
using System.Threading;
using OpenHardwareMonitor.Hardware;
using System.Linq;
using System.Text;

namespace TheXDS.Vulcanium.Arichua
{
    internal static class Program
    {
        private static readonly IEnumerable<Hammer> hammers = DetectHammers();
        private static readonly Stopwatch time = new Stopwatch();
        private static readonly System.Timers.Timer statusReport = new  System.Timers.Timer(1000.0);
        private static int? tempsDspLine = null;
        private static object _syncLock = new object();

        private static async Task Main()
        {
            SetPriority(ProcessPriorityClass.Idle);
            var tbreak = GetBreak(out var breaker);
            Console.CancelKeyPress += (_, e) => breaker!.Cancel();
            statusReport.Elapsed += OnReportTemps;
            await Task.WhenAny(Task.WhenAll(RunTortures()), tbreak);
            StopTortures();
            Environment.Exit(0);
        }

        private static Task GetBreak(out CancellationTokenSource breaker)
        {
            breaker = new CancellationTokenSource();
            var tcs = new TaskCompletionSource<object>();
            breaker.Token.Register(() => tcs.TrySetCanceled(), false);
            return tcs.Task;
        }


        private static void ShowMsj(string message, int row)
        {
            lock (_syncLock)
            {
                var oldLin = Console.CursorTop;
                var oldCol = Console.CursorLeft;
                Console.CursorTop = row;
                Console.CursorLeft = 0;
                Console.Write(message);
                Console.CursorTop = oldLin;
                Console.CursorLeft = oldCol;
            }
        }

        private static void OnReportTemps(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var s = new StringBuilder();
                var u = new UpdateVisitor();
                var c = new Computer();
                c.Open();
                c.CPUEnabled = true;
                c.Accept(u);
                foreach (var h in GetSensors(c))
                {
                    s.AppendLine($"{h.Name}: {h.Value} {Label(h.SensorType)} (Min: {h.Min} Max:{h.Max})".PadRight(Console.BufferWidth-1));
                }
                tempsDspLine ??= Console.CursorTop;
                c.Close();
                ShowMsj(s.ToString(), tempsDspLine.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"/!\\ {ex.Message}");
                statusReport.Stop();                
            }
        }

        private static string Label(SensorType s)
        {
            return s switch
            {
                SensorType.Clock => "MHz",
                SensorType.Temperature => "°C",
                SensorType.Power => "Watts",
                SensorType.Load => "%",
                SensorType.Voltage => "V",
                SensorType.Fan => "RPM",
                _=> s.ToString()
            };
        }

        private static IEnumerable<ISensor> GetSensors(IComputer c)
        {
            static bool IncludeSensor(IHardware hw)
            {
                switch (hw.HardwareType){
                    case HardwareType.CPU:
                    case HardwareType.RAM:
                        return true;
                    default: return false;
                }
            }
            return c.Hardware.Where(IncludeSensor).SelectMany(p => p.Sensors);
        }

        private static IEnumerable<Task> RunTortures()
        {
            return RunTortures(null);
        }

        private static IEnumerable<Task> RunTortures(TimeSpan? span)
        {
            var tasks = new List<Task>();
            statusReport.Start();
            time.Start();
            Console.WriteLine($"Tortura iniciada el {DateTime.Now}");
            foreach (var j in hammers)
            {
                tasks.Add(span is { } t ? j.Torture(t) : j.Torture());
            }
            return tasks;
        }

        private static void StopTortures()
        {
            Console.WriteLine("Deteniendo tests de tortura...");
            foreach (var h in hammers) h.Abort();
            time.Stop();
            statusReport.Stop();
            Console.WriteLine($"Tortura finalizada el {DateTime.Now}, tiempo total: {time.Elapsed}");
        }

        private static IEnumerable<Hammer> DetectHammers()
        {
            var hammers = new List<Hammer>();
            Console.WriteLine("Detectando pruebas de tortura...");

            foreach (var t in typeof(Program).Assembly.GetTypes())
            {
                if (TryInstance<Hammer>(t, out var test))
                {
                    Console.WriteLine($" - {t.Name} detectado.");
                    hammers.Add(test);
                }
            }

            return hammers;
        }

        private static void SetPriority(ProcessPriorityClass priority)
        {
            var thisProcess = Process.GetCurrentProcess();
            try
            {
                thisProcess.PriorityClass = priority;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No fue posible cambiar la prioridad de esta aplicación: {ex.Message}. Continuando con prioridad {thisProcess.PriorityClass}");
            }
        }

        private static bool TryInstance<T>(this Type t, out T instance)
        {
            if (t is null) throw new ArgumentNullException(nameof(t));
            if (!t.IsAbstract && !t.IsInterface &&
                typeof(T).IsAssignableFrom(t) &&
                t.GetConstructor(Type.EmptyTypes) is System.Reflection.ConstructorInfo ctor)
            {
                try
                {
                    instance = (T)ctor.Invoke(null);
                    return true;
                }
                catch { }
            }
            instance = default!;
            return false;
        }


        private class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }
    }
}