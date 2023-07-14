using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Datas.Entity.Player;
namespace Datas.DataRepository
{
    public class DataRepository
    {
        /** Entityのリスト */
        public PlayerEntity player;
        /** コンストラクター */
        public DataRepository()
        {
            player = InitializePlayer();
        }
        /** 初期化 */
        public PlayerEntity InitializePlayer()
        {
            var player = new PlayerEntity(5f,8f,5,5,5,8f);
    
            return player;
        }
    }
}

