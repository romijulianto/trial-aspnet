namespace ASPNETMaker2023.Models;

// Partial class
public partial class trial_05082023 {
    // Parameter for NpgsqlDbType.Citext
    class NpgsqlCitextParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly string? _value;

        private bool _nullable = true;

        public string? Value => _value;

        public NpgsqlCitextParameter(string? value, bool nullable = true)
        {
            if (!Empty(value))
                _value = value.ToString();
            _nullable = nullable;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            command.Parameters.Add(new NpgsqlParameter
            {
                ParameterName = name,
                NpgsqlDbType = NpgsqlDbType.Citext,
                Value = _value != null ? _value : (_nullable ? DBNull.Value : "")
            });
        }
    }

    // Parameter for NpgsqlDbType.Bit
    class NpgsqlBitParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly bool? _value;

        private bool _nullable = true;

        public bool? Value => _value;

        public NpgsqlBitParameter(bool? value, bool nullable = true)
        {
            if (!Empty(value))
                _value = value;
            _nullable = nullable;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            command.Parameters.Add(new NpgsqlParameter
            {
                ParameterName = name,
                NpgsqlDbType = NpgsqlDbType.Bit,
                Value = _value != null ? _value : (_nullable ? DBNull.Value : false)
            });
        }
    }

    // Parameter for NpgsqlDbType.Xml
    class NpgsqlXmlParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly string? _value;

        private bool _nullable = true;

        public string? Value => _value;

        public NpgsqlXmlParameter(string? value, bool nullable = true)
        {
            if (!Empty(value))
                _value = value;
            _nullable = nullable;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            command.Parameters.Add(new NpgsqlParameter
            {
                ParameterName = name,
                NpgsqlDbType = NpgsqlDbType.Xml,
                Value = _value != null ? _value : (_nullable ? DBNull.Value : "")
            });
        }
    }

    // Parameter for NpgsqlDbType.Json
    class NpgsqlJsonParameter : SqlMapper.ICustomQueryParameter
    {
        private readonly string? _value;

        private bool _nullable = true;

        public string? Value => _value;

        public NpgsqlJsonParameter(string? value, bool nullable = true)
        {
            if (!Empty(value))
                _value = value;
            _nullable = nullable;
        }

        public void AddParameter(IDbCommand command, string name)
        {
            command.Parameters.Add(new NpgsqlParameter
            {
                ParameterName = name,
                NpgsqlDbType = NpgsqlDbType.Json,
                Value = _value != null ? _value : (_nullable ? DBNull.Value : "null")
            });
        }
    }
} // End Partial class
