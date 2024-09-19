using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    // �u�������蔻��
    public GameObject triggerObject;

    private void Start()
    {
        replaceTilemap();
    }

    public void replaceTilemap()
    {
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            // ���o�����ʒu��񂩂�^�C���}�b�v�p�̈ʒu���(�Z�����W)���擾
            Vector3Int cellPosition = new Vector3Int(pos.x, pos.y, pos.z);

            // tilemap.HasTile -> �^�C�����ݒ�(�`��)����Ă�����W�ł��邩����
            if (tilemap.HasTile(cellPosition))
            {
                // �X�v���C�g����v���Ă��邩����
                if (tilemap.GetSprite(cellPosition) == sprite)
                {
                    // ����̃X�v���C�g�ƈ�v���Ă���ꍇ
                    Vector3 world_pos = tilemap.CellToWorld(cellPosition);
                    world_pos.x += 0.5f;
                    var obj =Instantiate(triggerObject, world_pos, Quaternion.identity);
                    obj.SetActive(true);
                }
            }
        }
    }
}
