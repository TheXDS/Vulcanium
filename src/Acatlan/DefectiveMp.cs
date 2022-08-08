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
V U L C A N I U M - A C A T L A N
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

namespace TheXDS.Vulcanium.Acatlan
{
    internal class DefectiveMp : ParallelMpTest
    {
        public override string Name => "Multihilo inseguro completamente defectuoso";

        public override string Description => @"
Esta prueba ejecutará una operación de conteo de enteros multi-hilo sin bloqueo
de recursos. La cuenta podría ser distinta con el mismo set de ejecución,
debido a que intencionalmente no se bloquea el acceso al contador por todos los
hilos creados. Si el resultado es distinto de una ejecución uni-thread, esto
podría significar un Crash en una aplicación real.

Esta implementación defectuosa de Multi-treading es únicamente para propósitos
de demostración y prueba.";

        public override void Run(int[] array)
        {
            base.Run(array);
            Count = _c;
        }

        protected override void ItemAction(int j)
        {
            if (Magma.IsPrime(j)) _c++;
        }

        private int _c = 0;

    }
    
        internal class VolatileMp : ParallelMpTest
    {
        public override string Name => "Multihilo volátil";

        public override string Description => @"
Esta prueba ejecuta una operación de conteo multi-hilo sin proteger el acceso
desde varios hilos, pero declarando la variable como volátil. El efecto de esta
declaración, es que el compilador elimina ciertas optimizaciones de acceso, de
modo que, aunque esta implementación sigue siendo defectuosa, tiene mejores
resultados que la operación multi-hilo desprotegida regular.

Esta implementación defectuosa de Multi-treading es únicamente para propósitos
de demostración y prueba.";

        public override void Run(int[] array)
        {
            base.Run(array);
            Count = _c;
        }

        protected override void ItemAction(int j)
        {
            if (Magma.IsPrime(j)) _c++;
        }

        private volatile int _c = 0;

    }
}
