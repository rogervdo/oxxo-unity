using UnityEngine;
using System.Collections.Generic;

public static class LogrosData
{
    public static List<LogroDTO> todosLosLogros = new List<LogroDTO>
    {
        new LogroDTO { id_logro = 1, nombre = "Explorador OXXO", descripcion = "Completar los 3 minijuegos.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro1"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro1_Bloqueado") },
        new LogroDTO { id_logro = 2, nombre = "Puntuación Galáctica", descripcion = "Sumar más de 1000 puntos.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro2"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro2_Bloqueado")  },
        new LogroDTO { id_logro = 3, nombre = "Asesor Premium", descripcion = "Obtener un puntaje mayor a 60 en Asesoría Express.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro3"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro3_Bloqueado")  },
        new LogroDTO { id_logro = 4, nombre = "Asesor Relámpago", descripcion = "Resolver todos los casos sin agotar el temporizador.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro4"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro4_Bloqueado")  },
        new LogroDTO { id_logro = 5, nombre = "Piloto Intocable", descripcion = "Completar Ox Spedition sin recibir daño.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro5"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro5_Bloqueado")  },
        new LogroDTO { id_logro = 6, nombre = "Superviviente Estelar", descripcion = "Sobrevivir 180 segundos en Ox Spedition.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro6"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro6_Bloqueado")  },
        new LogroDTO { id_logro = 7, nombre = "Mente Galáctica", descripcion = "Obtener 60 puntos en el juego de preguntas de Ox Spedition.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro7"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro7_Bloqueado")  },
        new LogroDTO { id_logro = 8, nombre = "Guardia Nocturno", descripcion = "Obtener más de 100 puntos en El Turno Final.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro8"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro8_Bloqueado")  },
        new LogroDTO { id_logro = 9, nombre = "Investigador Maestro", descripcion = "Obtener más de 300 puntos en El Turno Final.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro9"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro9_Bloqueado")  },
        new LogroDTO { id_logro = 10, nombre = "Maestro OXXODISEA", descripcion = "Obtener todos los demás logros.", spriteOriginal = Resources.Load<Sprite>("LogrosSprites/Logro10"), spriteBloqueado = Resources.Load<Sprite>("LogrosSprites/Logro10_Bloqueado")  }
        
    };
}
