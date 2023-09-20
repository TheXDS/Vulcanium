# Momotombo
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
## Introducci�n
Momotombo emula una terminal de teletexto con conexi�n serial de baja velocidad, obteniendo datos a trav�s de `stdin` y devolvi�ndolos con retraso a `stdout`. Permite establecer el color y la velocidad en baudios por medio de argumentos.

Al ejecutar Momotombo, se pueden utilizar los siguientes argumentos:

- `--green`: Establece el color de la consola en verde.
- `--amber`: Establece el color de la consola en ocre, simulando el color �mbar.
- `--baud=<valor>`: Establece el valor de baudios a emular en `<valor>`. Si se omite, se establecer� en `300` de forma predeterminada.

Recuerde que este programa utiliza `stdin` como entrada, por lo que deber�a utilizar *piping* en la terminal, as�:
``` sh
programaquegeneramuchotexto.exe | Momotombo.exe --green --baud=600
```

> NOTA: Si ejecuta una aplicaci�n de int�rprete de comandos, Momotombo muy probablemente no mostrar� el prompt del int�rprete sino hasta que presione `intro`. Esto obedece al funcionamiento interno de cada int�rprete de comandos, no enviando la salida del prompt a `stdout` sino hasta despu�s de colocar el cursor en el lugar de entrada y abrir `stdin`. Adem�s, el int�rprete de comandos tendr� libertad de modificar el color de la terminal a voluntad, pudiendo posiblemente ignorar los par�metros de color.
