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

namespace TheXDS.Vulcanium.Arichua
{
    internal static class Program
    {
        private static readonly IEnumerable<Hammer> hammers = DetectHammers();
        private static readonly Stopwatch time = new Stopwatch();
        private static System.Timers.Timer? cpuReport = new  System.Timers.Timer(1000.0);
        private static PerformanceCounter? cpuCounter;
        private static PerformanceCounter? ramCounter;
        private static int? cpuDisplLine = null;

        private static async Task Main()
        {
            DetectCounters();
            SetPriority(ProcessPriorityClass.Idle);
            var tbreak = GetBreak(out var breaker);
            Console.CancelKeyPress += (_, e) => breaker!.Cancel();
            if (cpuReport is {}) cpuReport.Elapsed += OnReportCpuStatus;
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

        private static void DetectCounters()
        {
            Console.WriteLine("Detectando contadores de rendimiento, por favor, espere...");
            try
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            }
            catch (System.Exception ex)
            {
                cpuReport = null;
                Console.WriteLine($"/!\\ {ex.Message}");
            }
        }

        private static void OnReportCpuStatus(object sender, ElapsedEventArgs e)
        {
            var s = $"Uso de CPU: {cpuCounter?.NextValue().ToString("f1") ?? "??"}%, Memoria disponible: {ramCounter?.NextValue().ToString() ?? "???"} MiB".PadRight(Console.BufferWidth);
            if (cpuDisplLine is null)
            {
                cpuDisplLine ??= Console.CursorTop;
                Console.WriteLine(s);
            }
            else
            {
                var oldLin = Console.CursorTop;
                var oldCol = Console.CursorLeft;
                Console.CursorTop = cpuDisplLine.Value;
                Console.CursorLeft = 0;
                Console.Write(s);
                Console.CursorTop = oldLin;
                Console.CursorLeft = oldCol;
            }
        }

        private static IEnumerable<Task> RunTortures()
        {
            return RunTortures(null);
        }

        private static IEnumerable<Task> RunTortures(TimeSpan? span)
        {
            var tasks = new List<Task>();
            cpuReport?.Start();
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
            cpuReport?.Stop();
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
    }
}