using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InGame.Model.Player
{
    public class PlayerModel
    {
        public float Speed;
        public float SprintSpeed;
        public int Attack;
        public int Defense;
        public int HP;
        public float JumpPower;

        public PlayerModel()
        {
            Speed = 0;
            SprintSpeed = 0;
            Attack = 0;
            Defense = 0;
            HP = 0;
            JumpPower = 0;
        }
    }
}

