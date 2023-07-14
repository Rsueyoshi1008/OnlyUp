using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Datas.Entity.Player
{
    public class PlayerEntity
    {
        public float Speed;
        public float SprintSpeed;
        public int Attack;
        public int Defense;
        public int HP;
        public float JumpPower;

        public PlayerEntity(float speed, float sprintSpeed, int attack, int defense, int hp, float jumpPower)
        {
            Speed = speed;
            SprintSpeed = sprintSpeed;
            Attack = attack;
            Defense = defense;
            HP = hp;
            JumpPower = jumpPower;
        }
    }
}

