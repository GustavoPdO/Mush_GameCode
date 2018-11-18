using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))] //avisar que o script precisa de um component do tipo camera no objeto
public class MultipleTargetCameraController_v0 : MonoBehaviour
{
    //public
    public List<Transform> targets; //lista de targets/alvos da camera
    public Vector3 cameraOffset; //offset da camera //(para poder ajustar a posicao da camera se necessario)

    [Space(5)]
    [Range(0, 1)]
    public float smoothTime; //"tempo que leva" para reajustar a camera 

    [Space(5)]
    public float minZoom; //zoom minimo que a camera pode fazer
    public float maxZoom; //zoom maximo que a camera pode fazer

    [Space(5)]
    //[Tooltip("Distancia maxima dos personagens na cena a partir da qual a camera vai dar 100% de zoom-out.")] //para ajudar no inspector da unity
    //public float maxTargetsDistance; //distancia maxima dos personagens na cena a partir da qual a camera vai dar 100% de zoom out //(talvez substituir por porcentagem da largura da cena depois)
    public Transform scenarioStart; //referencia a um objeto que sera o delimitador do comeco do cenario (canto inferior esquerdo)
    public Transform scenarioEnd; //referencia a um objeto que sera o delimitador do fim do cenario (canto superior direito)
    [Range(0, 1)]
    public float scenarioWidthMultiplier; //em qual porcentagem do cenario a camera vai estar no tamanho maximo

    public float maxLerpZoomCamSpeed; //multiplicador de velocidade maximo que a camera pode assumir 

    //private
    private Camera cam; //referancia ao componente Camera no objeto

    //private Vector3 centerPoint; //ponto central onde a camera vai focar
    private Bounds bounds; //caixa que vai receber grupo de objetos targets/alvos //(para uso na posicao/distancia da camera)
    private float scenarioWidth; //(para receber a diferenca do scenarioStart e scenarioEnd em x)
    private float scenarioLenght; //(para receber a diferenca do scenarioStart e scenarioEnd em y)

    private Vector3 smoothDampVelocity; //variavel utilizada pela funcao Vector3.SmoothDamp()

    public List<Transform> targetsWithRestrictor; //lista de targets/alvos da camera com o restritor de movimento da camera (para nao sair do cenario)
    private GameObject cameraMovementRestrictor;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>(); //inicializar valores

        //centerPoint = Vector3.zero; //inicializar valores
        bounds = new Bounds(Vector3.zero, Vector3.zero);
        scenarioWidth = Mathf.Abs(scenarioStart.position.x - scenarioEnd.position.x);
        scenarioLenght = Mathf.Abs(scenarioStart.position.y - scenarioEnd.position.y);

        cameraMovementRestrictor = new GameObject("CameraMovementRestrictor"); //inicializar restritor da camera para movimento
        cameraMovementRestrictor.transform.position = Vector3.zero;
        targetsWithRestrictor = new List<Transform>(targets);
        targetsWithRestrictor.Add(cameraMovementRestrictor.transform);

        Debug.Log("Scenario W: " + scenarioWidth + ". Scenario L: " + scenarioLenght);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //fisica
    private void FixedUpdate() //mais suave que o late update para a camera //(provavelmente por que o movimento do player se encontra no fixedUpdate)
    {
        if (targetsWithRestrictor.Count < 2) return; //caso nao tenha targets/alvos, nao mover camera/nao fazer nada (excluindo o restritor da camera)

        Debug.Log("Characters distance: " + Mathf.Abs(targets[0].position.x - targets[1].position.x));
        //+ ". Restrictor real position: " + gameObject.transform.position.x * ((minZoom - cam.fieldOfView) / scenarioWidth));

        //float xDistanceCenterTarget0 = Mathf.Abs(targets[0].position.x - (scenarioStart.position.x + scenarioWidth / 2)); //diferenca de distancia dos targets pro centro do cenario //(depois selecionar 2 targets mais longe do centro para funcionar para mais personagens na tela ao mesmo tempo)
        //float xDistanceCenterTarget1 = Mathf.Abs(targets[1].position.x - (scenarioStart.position.x + scenarioWidth / 2)); //diferenca de distancia dos targets pro centro do cenario
        //float xDistanceCenterDifference = (xDistanceCenterTarget0 - xDistanceCenterTarget1); //diferenca das distancias do centro dos targets

        //testando (ainda nao terminado)
        float restrictorNewPositionX = (gameObject.transform.position.x) * ((minZoom - cam.fieldOfView) / scenarioWidth); //posicao em x da camera * proporcao da abertura da camera pela largura do cenario
        cameraMovementRestrictor.transform.position = new Vector3(restrictorNewPositionX, 0, 0); //setar nova posicao do restritor antes de calcular nova posicao da camera

        SetTargetsBounds(targetsWithRestrictor); //setar bounds para fazer os calculos/usos necessarios para movimentar a camera
        MoveCamera(); //mover camera de acordo com a posicao dos targets

        if (targets.Count < 1) return; //caso nao tenha targets/alvos, nao mover camera/nao fazer nada

        SetTargetsBounds(targets); //setar bounds para fazer os calculos/usos necessarios para dar zoom na camera
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
    }

    public void SetTargetsBounds(List<Transform> targets) //setar bounds para fazer os calculos necessarios
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
