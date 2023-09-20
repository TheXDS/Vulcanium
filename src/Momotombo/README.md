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
## Introducción
Momotombo emula una terminal de teletexto con conexión serial de baja velocidad, obteniendo datos a través de `stdin` y devolviéndolos con retraso a `stdout`. Permite establecer el color y la velocidad en baudios por medio de argumentos.

Al ejecutar Momotombo, se pueden utilizar los siguientes argumentos:

- `--green`: Establece el color de la consola en verde.
- `--amber`: Establece el color de la consola en ocre, simulando el color ámbar.
- `--baud=<valor>`: Establece el valor de baudios a emular en `<valor>`. Si se omite, se establecerá en `300` de forma predeterminada.

Recuerde que este programa utiliza `stdin` como entrada, por lo que debería utilizar *piping* en la terminal, así:
``` sh
programaquegeneramuchotexto.exe | Momotombo.exe --green --baud=600
```

> NOTA: Si ejecuta una aplicación de intérprete de comandos, Momotombo muy probablemente no mostrará el prompt del intérprete sino hasta que presione `intro`. Esto obedece al funcionamiento interno de cada intérprete de comandos, no enviando la salida del prompt a `stdout` sino hasta después de colocar el cursor en el lugar de entrada y abrir `stdin`. Además, el intérprete de comandos tendrá libertad de modificar el color de la terminal a voluntad, pudiendo posiblemente ignorar los parámetros de color.
