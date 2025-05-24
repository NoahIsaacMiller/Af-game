using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyGame
{
    public class OnScenceSetup : MonoBehaviour
    {
        private float _sceneLoadTime;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        }

        void Awake()
        {
            SceneManager.sceneLoaded += (scene, loadMode) =>
            {
                this._sceneLoadTime = Time.time;
            };
        }

        // Update is called once per frame
        void Update()
        {

        }

        public float timeConsuming()
        {
            return Time.time - this._sceneLoadTime;
        }
    }

}