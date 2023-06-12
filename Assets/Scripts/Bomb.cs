using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    // variavel utilizada para a explosao
    public GameObject explosionPrefab;
    // Layer para o script da Bomb conseguir aceder
    public LayerMask levelMask;

    private bool exploded = false;

    // Start is called before the first frame update
    void Start()
    {
        // 3f é o tempo que demora a explodir, o "Explode" é o metodo escolhido
        Invoke("Explode", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Explode()
    {
        // Spawna uma explosão na posição da bomba
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // O startCouroutine vai iniciar o CreateExplosions IEnumerator uma vez em cada direção
        StartCoroutine(CreateExplosions(Vector3.forward));
        StartCoroutine(CreateExplosions(Vector3.right));
        StartCoroutine(CreateExplosions(Vector3.back));
        StartCoroutine(CreateExplosions(Vector3.left)); 


        // Desativa a "mesh" da bomba e torna-a invisivel
        GetComponent<MeshRenderer>().enabled = false; //2

        exploded = true;



        // Desativa o collider para os jogadores poderem passar pela explosão
        transform.Find("Collider").gameObject.SetActive(false); //3
        // Destroi a bomba passado 0.3 segundos, faz com que todas as explosões acontecam antes do GameObject seja destruido
        Destroy(gameObject, .3f); //4
    }

    private IEnumerator CreateExplosions(Vector3 direction)
    {
        
        //1 
        // Vai iterar por cada unidade de distancia que queremos que as explosões cubram, neste caso
        // as explosões vão alcançar dois metros
        for (int i = 1; i < 3; i++)
        {
        //2
        // O objeto guarda a informação sobre o quê e onde é que as explosões atingem, ou não.
        RaycastHit hit;
        //3
        //Envia um raycast do centro da bomba em direção às que passamos através do StartCoroutine
        // Depois faz o output do resultado do RaycastHit. O Parametro i dita a distancia que o ray
        // deve viajar. Finalmente usa a LayerMask chamada levelMask para ter a certeza que o raio
        // apenas atinge os blocos dentro do nivel e ignora os jogadores e os outros colliders
        Physics.Raycast(transform.position + new Vector3(0,.5f,0), direction, out hit, i, levelMask);
        //4
        // Se o raycast nao atingir nada entao é um free tile
        if (!hit.collider) 
        {
            // Spawna uma explosão na posição que o raycast checou
            Instantiate(explosionPrefab, transform.position + (i * direction), //5

            // o raycast atinge um bloco
            explosionPrefab.transform.rotation); //6

        } else 
        { 

            //7
            // mal atinja um bloco faz o break do loop para não permitir que a explosão 
            // vá por cima de paredes
            break; 
        }

        //8
        // espera 0.05 segundos antes de fazer a proxima iteração, isto permite que a explosão pareca
        // mais real fazendo com que se esteja a expandir para os lados.
        yield return new WaitForSeconds(.05f); 
        
        }
    }

    // É um metodo pre definido que é chamado quando existe uma colisão de um trigger collider e um rigidbody
    public void OnTriggerEnter(Collider other)
    {

        if(!exploded && other.CompareTag("Explosion"))
        {
            // Verifica se a bomba ainda não explodiu e verifica se o trigger collider tem a tag Explosion assigned
        CancelInvoke("Explode");

        Explode();
        }
        
    }    

}
