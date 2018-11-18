using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))] //avisar que o script precisa de um component do tipo camera no objeto //(adiciona automatico quando nao tem)
public class MultipleTargetCameraController : MonoBehaviour
{
    //public
    public List<Transform> targets; //lista de targets/alvos da camera
    public Vector3 cameraOffset; //offset da camera //(para poder ajustar a posicao da camera se necessario)

    [Space(5)]
    [Range(0, 1)]
    public float smoothTime; //"tempo que leva" para reajustar a camera 

    [Space(5)]
    public float maxZoom; //zoom maximo que a camera pode fazer ( < minZoom)

    [Space(5)]
    //[Tooltip("Distancia maxima dos personagens na cena a partir da qual a camera vai dar 100% de zoom-out.")] //para ajudar no inspector da unity
    //public float maxTargetsDistance; //distancia maxima dos personagens na cena a partir da qual a camera vai dar 100% de zoom out //(talvez substituir por porcentagem da largura da cena depois)
    public Transform scenarioStart; //referencia a um objeto que sera o delimitador do comeco do cenario (canto superior esquerdo)
    public Transform scenarioEnd; //referencia a um objeto que sera o delimitador do fim do cenario (canto inferior direito)
    [Range(0, 1)]
    public float scenarioWidthMultiplier; //em qual porcentagem do cenario a camera vai estar no tamanho maximo

    public float maxLerpZoomCamSpeed; //multiplicador de velocidade maximo que a camera pode assumir 

    //private
    private Camera cam; //referancia ao componente Camera no objeto

    private float minZoom; //zoom minimo que a camera pode fazer (vendo o cenario inteiro) ( > maxZoom)

    //private Vector3 centerPoint; //ponto central onde a camera vai focar
    private Bounds bounds; //caixa que vai receber grupo de objetos targets/alvos //(para uso na posicao/distancia da camera)
    private float scenarioWidth; //(para receber a diferenca do scenarioStart e scenarioEnd em x)
    private float scenarioHeight; //(para receber a diferenca do scenarioStart e scenarioEnd em y)

    private Vector3 smoothDampVelocity; //variavel utilizada pela funcao Vector3.SmoothDamp()

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>(); //inicializar valores

        //centerPoint = Vector3.zero; //inicializar valores
        bounds = new Bounds(Vector3.zero, Vector3.zero);
        scenarioWidth = Mathf.Abs(scenarioStart.position.x - scenarioEnd.position.x);
        scenarioHeight = Mathf.Abs(scenarioStart.position.y - scenarioEnd.position.y);

        Debug.Log("Scenario W: " + scenarioWidth + ". Scenario L: " + scenarioHeight);

        minZoom = 2.0f * Mathf.Atan((scenarioWidth / cam.aspect) * 0.5f / Mathf.Abs(cameraOffset.z)) * Mathf.Rad2Deg; //encontrar cam.fieldOfView (minZoom) da camera baseada no tamanho/na largura do cenario
        Debug.Log("Camera minZoom: " + minZoom);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //fisica
    private void FixedUpdate() //mais suave que o late update para a camera //(provavelmente por que o movimento do player se encontra no fixedUpdate)
    {
        if (targets.Count < 1) return; //caso nao tenha targets/alvos, nao mover camera/nao fazer nada

        SetTargetsBounds(); //setar bounds para fazer os calculos/usos necessarios
        MoveCamera(); //mover camera de acordo com a posicao dos targets
        ZoomCamera(); //ajustar camera de acordo com a posicao dos targets
    }

    ////atualizar ao final das chamadas de update
    //private void LateUpdate()
    //{

    //}

    public void ZoomCamera() //ajustar camera de acordo com a posicao dos targets
    {
        float boxWidth = bounds.size.x; //pegar tamanho/largura (width) da caixa (bounds) //(maior distancia entre targets/alvos)
        //Debug.Log("Targets distance: " + boxWidth); //para testes

        float newZoom = Mathf.Lerp(maxZoom, minZoom, boxWidth / (scenarioWidth * scenarioWidthMultiplier)); //pegar novo Zoom //(+ normalizar lerp com o boxWitdh/targetsDistanceStartZoom como se "maxZoom = 0 , minZoom = 1")

        //Debug.Log("fov-zoom: " + (cam.fieldOfView - newZoom)); //para testes
        float zoomDifference = Mathf.Abs(cam.fieldOfView - newZoom); //pegar diferenca absoluta do zoom atual e o zoom destino
        if (zoomDifference < 0.01) //se a diferenca do zoom atual e do que pretende alcancar estiver entre ]-0.01, 0.01[, aproximar
        {
            cam.fieldOfView = newZoom; //aproximar zoom
        }
        else //senao, realizar interpolacao para aproximar mais os valores
        {
            float lerpCamSpeed = Mathf.Lerp(1, maxLerpZoomCamSpeed, zoomDifference / (minZoom - maxZoom)); //variar velocidade de reajuste da camera de acordo com a distancia do zoom atual ao zoom desejado

            //Debug.Log("fov: " + cam.fieldOfView + " - nz: " + zoomDifference / (minZoom - maxZoom)); //para testes
            //Debug.Log("lerpCamSpeed: " + lerpCamSpeed + " - tdt: " + Time.deltaTime); //para testes
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime * lerpCamSpeed); //ajustar lentamente novo Zoom
        }
    }

    public void MoveCamera() //mover camera de acordo com a posicao dos targets
    {
        Vector3 centerPoint = bounds.center; //pegar ponto central entre os targes/alvos da camera
        Vector3 newPosition = centerPoint + cameraOffset; //nova posicao = centerPoint + offset

        //transform.position = centerPoint + cameraOffset; //setar nova posicao da camera + offset
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref smoothDampVelocity, smoothTime); //setar lentamente nova posicao da camera + offset

        ClampCameraOnScenario(); // "prende" a camera em perspectiva no cenario (cenario tem que ser proporcional ao campo de visao para comportar a camera no minZoom)
    }

    public void ClampCameraOnScenario() //"prende" a camera em perspectiva no cenario //(cenario tem que ser proporcional ao campo de visao para comportar a camera no minZoom) //ex: (cenario (16:9) * x , x = 1, 2, etc.)
    {
        //Horizontal
        float scenarioCenterWidth = (scenarioStart.position.x + scenarioWidth / 2); //posicao real do centro do cenario na horizontal

        float scenarioEndWidth = (scenarioCenterWidth > cam.transform.position.x) ? scenarioStart.position.x : scenarioEnd.position.x; //verificar qual a "parede" mais proxima (em largura/na horizontal) da camera (esquerda ou direita) para realizar os ajustes de posicao da camera
        float distanceCameraEndOfScreenWidth = Mathf.Abs(scenarioEndWidth - cam.transform.position.x); // distancia da camera ao final da tela na direcao mais proxima (na horizontal)

        //(+ descobrir tamanho real da camera com o cenario) //feito
        float distanceCameraEndOfCameraWidth = GetCameraFrustrum().x / 2; // distancia da camera ao final da tela (na horizontal) (igual para ambos os lados da tela) //(acertar distancia, colocar para ambos os lados do cenario, etc.) //ajustado

        //Debug.Log(distanceCameraEndOfScreenWidth + " s:c " + distanceCameraEndOfCameraWidth);
        float camNewX = transform.position.x;
        if (distanceCameraEndOfScreenWidth < distanceCameraEndOfCameraWidth)
        {
            camNewX = transform.position.x - (Mathf.Abs(distanceCameraEndOfCameraWidth - distanceCameraEndOfScreenWidth) * (scenarioEndWidth == scenarioStart.position.x ? -1 : 1)); //encontrar novo x da camera ("-" porque o start comeca na esquerda)
        }

        //Vertical
        float scenarioCenterHeight = (scenarioStart.position.y - scenarioHeight / 2); //posicao real do centro do cenario na vertical

        float scenarioEndHeight = (scenarioCenterHeight < cam.transform.position.y) ? scenarioStart.position.y : scenarioEnd.position.y; //verificar qual a "parede" mais proxima (em altura/na vertical) da camera (em cima ou em baixo) para realizar os ajustes de posicao da camera
        float distanceCameraEndOfScreenHeight = Mathf.Abs(scenarioEndHeight - cam.transform.position.y); // distancia da camera ao final da tela na direcao mais proxima (na vertical)

        float distanceCameraEndOfCameraHeight = GetCameraFrustrum().y / 2; // distancia da camera ao final da tela (na vertical) (igual para ambos os lados da tela)

        //Debug.Log(distanceCameraEndOfScreenHeight + " s:c " + distanceCameraEndOfCameraHeight);
        float camNewY = transform.position.y;
        if (distanceCameraEndOfScreenHeight < distanceCameraEndOfCameraHeight)
        {
            camNewY = transform.position.y + (Mathf.Abs(distanceCameraEndOfCameraHeight - distanceCameraEndOfScreenHeight) * (scenarioEndHeight == scenarioStart.position.y ? -1 : 1)); //encontrar novo y da camera ("+" porque o start comeca em cima)
        }

        transform.position = new Vector3(camNewX, camNewY, transform.position.z); //setar novas posicoes da camera
    }

    public Vector2 GetCameraFrustrum() //retorna um par (Width, Height) que eh o tamanho campo visivel da camera (largura e altura) [para uma camera em distancia reta do plano -> dist = |z|]
    {
        float frustrumHeight = 2.0f * Mathf.Abs(cameraOffset.z) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad); //pegar altura visivel da camera
        float frustrumWidth = frustrumHeight * cam.aspect; //pegar largura visivel da camera

        //Debug.Log("CameraHeight: " + frustrumHeight + ". CameraWidth: " + frustrumWidth);
        return new Vector2(frustrumWidth, frustrumHeight);
    }

    public void SetTargetsBounds() //setar bounds para fazer os calculos necessarios
    {
        //if (targets.Count < 1) return Vector3.zero; //caso nao tenha targets/alvos
        //if (targets.Count == 1) return targets[0].position; //caso seja apenas um target/alvo

        bounds = new Bounds(targets[0].position, Vector3.zero); //criar uma caixa centrada to primeiro target/alvo
        foreach (Transform t in targets) //para cada transform no vetor targets
        {
            bounds.Encapsulate(t.position); //adicionar/aumentar a caixa para incluir o novo ponto
        }
        //return bounds.center; //retornar centro da caixa gerada com todos os pontos
    }
}
