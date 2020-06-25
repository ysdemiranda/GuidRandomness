using System;

namespace GuidRandomness.Model
{
    public struct SegmentedGuid
    {
        public string original { get; set; }
        public Int32 segment_a { get; set; }
        public Int16 segment_b { get; set; }
        public Int16 segment_c { get; set; }
        public Int16 segment_d { get; set; }
        public Int64 segment_e { get; set; }

        public SegmentedGuid(Guid guid)
        {
            original = guid.ToString();
            string[] segments = guid.ToString().Split('-');
            segment_a = Int32.Parse(segments[0], System.Globalization.NumberStyles.HexNumber);
            segment_b = Int16.Parse(segments[1], System.Globalization.NumberStyles.HexNumber);
            segment_c = Int16.Parse(segments[2], System.Globalization.NumberStyles.HexNumber);
            segment_d = Int16.Parse(segments[3], System.Globalization.NumberStyles.HexNumber);
            segment_e = Int64.Parse(segments[4], System.Globalization.NumberStyles.HexNumber);
        }

    }

    public struct WideGuid
    {
        public Int64 segment_a { get; set; }
        public Int32 segment_b { get; set; }
        public Int32 segment_c { get; set; }
        public Int32 segment_d { get; set; }
        public Int64 segment_e { get; set; }

    }

    public class GuidSampleModel
    {
        private Guid _content;
        public SegmentedGuid Content
        {
            get { return new SegmentedGuid(_content); }
        }

        private uint _sampleIndex;
        public uint Index
        {
            get { return _sampleIndex; }
        }

        public GuidSampleModel(uint index)
        {
            _sampleIndex = index;
            _content = Guid.NewGuid();
        }
    }
}