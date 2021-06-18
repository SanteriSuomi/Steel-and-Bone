using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimator : Singleton<PlayerAnimator>
{
    public Animator Animator { get; private set; }
    private string[] animParams;

    protected override void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SCE_Dungeon")
        {
            Animator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
            InitializeParams();
        }
    }

    private void InitializeParams()
    {
        animParams = new string[Animator.parameterCount];
        for (int i = 0; i < Animator.parameterCount; i++)
        {
            animParams[i] = Animator.GetParameter(i).name;
        }
    }

    /// <summary>
    /// Disable all other animator parameters except the one specified in the parameter of this method.
    /// </summary>
    /// <param name="nameOfParameterToEnable"></param>
    public void ResetAnimatorParamsExcept(string nameOfParameterToEnable)
    {
        for (int i = 0; i < animParams.Length; i++)
        {
            if (animParams[i] == nameOfParameterToEnable)
            {
                Animator.SetBool(animParams[i], true);
            }
            else
            {
                Animator.SetBool(animParams[i], false);
            }
        }
    }
}