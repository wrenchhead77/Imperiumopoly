using System.Collections;
using UnityEngine;

public class MovementMgr : MonoBehaviour
{
    public static MovementMgr Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public IEnumerator MovePlayerToPosition(GameObject playerPiece, Vector3[] positions, float moveSpeed)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            Vector3 startPosition = playerPiece.transform.position;
            Vector3 targetPosition = positions[i];

            float elapsedTime = 0f;
            while (elapsedTime < moveSpeed)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / moveSpeed;
                playerPiece.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
        }
    }

    public void SetPlayerPosition(GameObject playerPiece, Vector3 position)
    {
        playerPiece.transform.position = position;
    }
}
