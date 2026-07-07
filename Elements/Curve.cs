using System;

namespace EpanetSharp.Elements
{
    /// <summary>
    /// Representa uma curva (por exemplo, curva de bomba, curva de perda) utilizada na rede.
    /// Esta classe é abstrata e contém propriedades comuns a diferentes tipos de curvas.
    /// </summary>
    public abstract class Curve : NetworkElement
    {
    }
}
