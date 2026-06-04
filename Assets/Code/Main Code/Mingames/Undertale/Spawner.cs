using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    private float intervalo, angle, speed;
    private int level;
    private Vector3 pos, scale;
    private Vector2 dir;
    private Quaternion rotation;
    private Abejas nuevaAbeja;
    private GameObject go;
    public Abejas abeja;
    public ProtaUndertale prota;


    void Start()
    {
        //Las siguientes 4 variables son para iniciar cada abeja, angle se explica más tarde
        dir.x = 0f;
        dir.y = 0f;
        angle = 0f;
        speed = 2f;
        level = 1;//Indica el nivel en el que se encuentra el jugador
        intervalo = 0.5f;//Es una variable que utilizo para algunos niveles, la explico después
        //Las siguientes dos líneas son para el nivel 5
        go = GameObject.FindWithTag("Player");
        prota = go.GetComponent<ProtaUndertale>();
    }

    public void startGame()
    {
        StartCoroutine(Level1());//Se inicia el timer con el primer nivel
    }


    //Después del primer nivel se van iniciando el resto de timers de cada nivel en orden, esta función se activa desde la prota
    public void NextLevel()
    {
        ++level;
        if (level == 2)
        {
            StartCoroutine(Level2());
        }
        else if (level == 3)
        {
            StartCoroutine(Level3());
        }
        else if (level == 4)
        {
            StartCoroutine(Level4());
        }
        else if (level == 5)
        {
            StartCoroutine(Level5());
        }
    }

    //En los niveles se escriben a mano las posiciones desde dónde aparecen las abejas, sus direcciones y la velocidad a la que van (esto último nunca cambia)
    //En este nivel hago que aparezcan paredes de abejas intercaladas a izquierda y derecha
    IEnumerator Level1()
    {
        while (level == 1)
        {
            pos.x = -5f;
            dir.x = 1f;
            //Intervalo lo utilizo para que haya una diferencia en la posición del spawn entre una oleada y la siguiente
            if (intervalo == 0.5f) intervalo = 0f;
            else intervalo = 0.5f;

            //En este bucle calculo la altura y en cada ejecución invierto el spawn y dirección en x
            for (float i = 2.5f + intervalo; i > -3f; i -= 2f)
            {
                if (level != 1) break;
                pos.y = i;
                pos.x *= -1f;
                dir.x *= -1f;
                NuevaAbeja(pos, dir, speed);//Creo una nueva abeja
            }

            yield return new WaitForSeconds(2f);//Todo lo de arriba se ejecuta cada 2 segundos
        }
    }

    //En este nivel hago dos líneas de abejas que se mueven en diagonal
    IEnumerator Level2()
    {
        while (level == 2)
        {
            pos.x = -5f;
            dir.x = 1f;
            dir.y = 0.25f;
            if (intervalo == -1.5f) intervalo = 0f;
            else intervalo = -1.5f;

            //Aquí solo cambio la posicion del spawn en y
            for (float i = 0.5f + intervalo; i >= -3f; i -= 3f)
            {
                if (level != 2) break;
                pos.y = i;
                NuevaAbeja(pos, dir, speed);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    //En este nivel hago que aparezcan abejas abajo del pozo y suban hacia arriba
    IEnumerator Level3()
    {
        while (level == 3)
        {
            pos.y = -3f;
            dir.x = 0f;
            dir.y = 1f;
            if (intervalo == 1.5f) intervalo = 0f;
            else intervalo = 1.5f;

            //Aquí solo cambio la posición x del spawn
            for (float i = 2.5f + intervalo; i >= -4f; i -= 3f)
            {
                if (level != 3) break;
                pos.x = i;
                NuevaAbeja(pos, dir, speed);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    //Desde el centro del pozo de la izquierda aparecen abejas formando un patrón tipo disparo triple
    IEnumerator Level4()
    {
        pos.x = -4.5f;
        pos.y = 0f;
        dir.x = 1f;
        dir.y = -0.25f;
        speed = 2.5f;
        //Aquí dentro voy aumentado la dirección y hasta que llega al límite y vuelve al inicio para crear el disparo triple
        while (level == 4)
        {

            for (int i = 0; i < 3; i++)
            {
                if (level != 4) break;
                NuevaAbeja(pos, dir, speed);
                dir.y += 0.25f;
            }
            dir.y = -0.25f;

            yield return new WaitForSeconds(1.25f);
        }
    }

    //Del centro spawneo abejas que apuntan al jugador
    IEnumerator Level5()
    {
        while (level == 5)
        {
            //Creo una nueva abeja en el centro que apunta al jugador
            //Es el centro porque el spawner (transform.position) se encuentra en el centro
            speed = 4f;
            dir = (prota.transform.position - transform.position).normalized;
            pos = transform.position;
            NuevaAbeja(pos, dir, speed);

            yield return new WaitForSeconds(0.5f);//Todo lo de arriba se ejecuta cada 2 segundos
        }
    }

    void NuevaAbeja(Vector3 pos, Vector2 dir, float speed)//Función para spawnear abejas
    {
        //angle, rotation y scale lo utilizo para que los sprites de las abejas apunten hacia donde deben
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 180;
        rotation = Quaternion.Euler(0f, 0f, angle);

        nuevaAbeja = Instantiate(abeja, pos, rotation);

        scale = nuevaAbeja.transform.localScale;
        scale.y = Mathf.Abs(scale.y) * (dir.x > 0 ? -1f : 1f);
        nuevaAbeja.transform.localScale = scale;

        //Aquí pongo la dirección y velocidad de la abeja
        nuevaAbeja.moveDir.x = dir.x;
        nuevaAbeja.moveDir.y = dir.y;
        nuevaAbeja.moveSpeed = speed;
    }
}
