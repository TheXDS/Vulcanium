# Acatlan
```
  _.----._
 (   (    )
(  (    )  )
 (________)
    ||||
  --++++--
    ||||
  .(    ).
 (_(____)_)
```
## Introducción
Acatlan demuestra el efecto del uso incorrecto de operaciones multi-hilo sobre un mismo set de datos y sobre una misma variable de conteo.

Se incluyen algunas diferentes clases que ejecutan acciones multi-hilo:

- Unithread de referencia: Ejecuta una operación en un único hilo para obtener un valor correcto contra el cual comparar los otros métodos.
- Multi-hilo inseguro completamente defectuoso: Como su nombre lo indica, se trata de una operación multi-hilo donde no se toma ninguna consideración de seguridad de acceso. El valor calculado será distinto de la respuesta correcta. Entre más hilos de ejecución soporte un CPU, mayor será la desviación con respecto al valor esperado.
- Multi-hilo volátil: Igual que el multi-hilo inseguro, pero declarando el contador como volátil. El efecto de esta
declaración, es que el compilador elimina ciertas optimizaciones de acceso, de modo que, aunque esta implementación sigue siendo defectuosa, tiene resultados ligeramente más confiables que la operación multi-hilo insegura regular.
- Multi-hilo con syncLock: Ejecuta una operación donde cada hilo bloquea un objeto de sincronización cuando se desea acceder a un valor compartido por varios hilos. En consecuencia, los hilos esperarán a que se libere el mútex, asegurando que las operaciones de conteo no interfieran unas con las otras.
- Multi-hilo de bolsa concurrente: Una solución poco convencional al problema de sincronización. Utiliza la clase ```System.Collections.Concurrent.ConcurrentBag<T>``` donde cada hilo guardará resultados preeliminares. Al final de la operación, se devuelve la cuenta de elementos contenidos en la bolsa concurrente. Consume más memoria que otros métodos, pero expone todos los valores resultantes del cálculo lo cual, de acuerdo a los requerimientos del algoritmo, podría ser deseable.