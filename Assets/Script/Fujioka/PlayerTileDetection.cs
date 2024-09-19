using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTileDetection : MonoBehaviour
{
    public Tilemap tilemap; // タイルマップを指定
    public Vector3Int tilePositionOffset = Vector3Int.zero; // プレイヤーの中心からタイル位置を補正する場合

    void Update()
    {
        // プレイヤーの現在位置をタイルマップのグリッド座標に変換
        Vector3 worldPosition = transform.position;
        Vector3Int gridPosition = tilemap.WorldToCell(worldPosition + tilePositionOffset);

        // 現在のタイルを取得
        Sprite currentTile = tilemap.GetSprite(gridPosition);

        if (currentTile != null)
        {
            Debug.Log("プレイヤーが" + currentTile.name + "タイルを踏んだ");

            // タイルに基づいて特定の処理を実行
            if (currentTile.name == "SpecialTile")
            {
                // 特定のタイルを踏んだ場合の処理
                TriggerSpecialEvent();
            }
            transform.position = Vector3.zero;
        }
    }

    void TriggerSpecialEvent()
    {
        // 特定のタイルを踏んだ際の処理
        Debug.Log("特定のタイルを踏んだ！");
    }
}
