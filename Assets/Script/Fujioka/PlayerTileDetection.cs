using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerTileDetection : MonoBehaviour
{
    public Tilemap tilemap; // �^�C���}�b�v���w��
    public Vector3Int tilePositionOffset = Vector3Int.zero; // �v���C���[�̒��S����^�C���ʒu��␳����ꍇ

    void Update()
    {
        // �v���C���[�̌��݈ʒu���^�C���}�b�v�̃O���b�h���W�ɕϊ�
        Vector3 worldPosition = transform.position;
        Vector3Int gridPosition = tilemap.WorldToCell(worldPosition + tilePositionOffset);

        // ���݂̃^�C�����擾
        Sprite currentTile = tilemap.GetSprite(gridPosition);

        if (currentTile != null)
        {
            Debug.Log("�v���C���[��" + currentTile.name + "�^�C���𓥂�");

            // �^�C���Ɋ�Â��ē���̏��������s
            if (currentTile.name == "SpecialTile")
            {
                // ����̃^�C���𓥂񂾏ꍇ�̏���
                TriggerSpecialEvent();
            }
            transform.position = Vector3.zero;
        }
    }

    void TriggerSpecialEvent()
    {
        // ����̃^�C���𓥂񂾍ۂ̏���
        Debug.Log("����̃^�C���𓥂񂾁I");
    }
}
