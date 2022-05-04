﻿using EmpresariosConLiderazgo.Utils;

namespace EmpresariosConLiderazgo.Models.Entities
{
    public class Movements : BaseClass
    {
        public int IdBalanceProduct { get; set; }
        public DateTime DateMovement { get; set; }
        public float BalanceBefore { get; set; }
        public float CashOut { get; set; }
        public float BalanceAfter { get; set; }
        public EnumStatus status { get; set; }
    }
}
