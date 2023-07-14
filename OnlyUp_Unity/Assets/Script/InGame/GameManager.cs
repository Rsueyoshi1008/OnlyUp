using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Datas.DataRepository;
public class GameManager : MonoBehaviour
{
    private static DataRepository _dataRepository;
    public DataRepository GetDataRepository()
    {
        if (_dataRepository == null)
        {
            _dataRepository = new DataRepository();
        }
        return _dataRepository;
    }
    /** シーンの遷移 */
    public void TransitionScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
