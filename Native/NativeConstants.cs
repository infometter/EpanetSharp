namespace EpanetSharp.Native
{
    /// <summary>
    /// Constantes utilizadas com EN_getcount na API nativa do EPANET.
    /// Os valores representam códigos de contagem conforme a API C do EPANET.
    /// </summary>
    internal static class NativeConstants
    {
        public const int EN_NODECOUNT = 0;
        public const int EN_LINKCOUNT = 1;
        public const int EN_TANKCOUNT = 2;
        public const int EN_RESERVOIRCOUNT = 3;
        public const int EN_PATTERNCOUNT = 4;
        public const int EN_CURVECOUNT = 5;
        public const int EN_CONTROLCOUNT = 6;

        // Option codes (EN_getoption/EN_setoption) - values are from EPANET C API
        // Prefixed with EN_OPTION_ to avoid name collisions with node/link parameter codes.
        public const int EN_OPTION_UNITS = 0;
        public const int EN_OPTION_HEADLOSS = 1;
        public const int EN_OPTION_DEMANDMULT = 2;
        public const int EN_OPTION_VISCOSITY = 3;
        public const int EN_OPTION_SPECGRAV = 4;
        public const int EN_OPTION_ACCURACY = 5;
        public const int EN_OPTION_TRIALS = 6;
        public const int EN_OPTION_PATTERN = 7;
        public const int EN_OPTION_DURATION = 8;
        public const int EN_OPTION_HYDSTEP = 9; // Time step

        // Node parameter codes (from EPANET C API)
        public const int EN_ELEVATION = 0;
        public const int EN_BASEDEMAND = 1;
        public const int EN_PATTERN = 2;
        public const int EN_EMITTER = 3;
        public const int EN_INITQUAL = 4; // initial quality / source quality
        public const int EN_SOURCE_QUAL = 5;
        public const int EN_STATUS = 6;
        public const int EN_TAG = 7;
        public const int EN_COMMENT = 8;

        // Additional node param codes (may map to EPANET reporting parameters)
        public const int EN_PRESSURE = 9;
        public const int EN_HEAD = 10;
        public const int EN_QUALITY = 11;

        // Link parameter codes (assumed mapping) - prefixed to avoid name collisions
        public const int EN_LINK_LENGTH = 0;
        public const int EN_LINK_DIAMETER = 1;
        public const int EN_LINK_ROUGHNESS = 2;
        public const int EN_LINK_MINORLOSS = 3;
        public const int EN_LINK_STATUS = 4;
        public const int EN_LINK_FLOW = 5;
        public const int EN_LINK_VELOCITY = 6;
        public const int EN_LINK_HEADLOSS = 7;
        public const int EN_LINK_POWER = 8;
        public const int EN_LINK_SPEED = 9;
        public const int EN_LINK_ENERGY = 10;
        public const int EN_LINK_PATTERN = 11;
        public const int EN_LINK_CURVE = 12;
        // Generic link parameters for valves
        public const int EN_LINK_SETTING = 13;
    }
}
