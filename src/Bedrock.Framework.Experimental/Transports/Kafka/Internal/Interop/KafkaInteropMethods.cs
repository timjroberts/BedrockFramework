using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Bedrock.Framework.Kafka.Internal.Interop
{
    public static class KafkaInteropMethods
    {
        public const int MaxErrorStringLength = 512;

        private const string KafkaDllName = "librdkafka";

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr rd_kafka_version_str();

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern KafkaSafeHandle rd_kafka_new(RdKafkaType type, IntPtr conf, StringBuilder errstr, UIntPtr errstr_size);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rd_kafka_destroy(IntPtr rk);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern KafkaConfigSafeHandle rd_kafka_conf_new();

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern RdKafkaConfRes rd_kafka_conf_set(
            IntPtr conf,
            [MarshalAs(UnmanagedType.LPStr)] string name,
            [MarshalAs(UnmanagedType.LPStr)] string value,
            StringBuilder errstr,
            UIntPtr errstr_size);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern RdKafkaErrorCode rd_kafka_consumer_close(IntPtr rk);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rd_kafka_conf_destroy(IntPtr conf);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern RdKafkaErrorCode rd_kafka_subscribe(IntPtr rk, IntPtr topics);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern RdKafkaErrorCode rd_kafka_unsubscribe(IntPtr rk);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr rd_kafka_topic_partition_list_new(IntPtr size);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void rd_kafka_topic_partition_list_destroy(IntPtr rkparlist);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr rd_kafka_topic_partition_list_add(IntPtr rktparlist, string topic, int partition);

        [DllImport(KafkaDllName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr rd_kafka_consumer_poll(IntPtr rk, IntPtr timeout_ms);
    }

    public enum RdKafkaType
    {
        RD_KAFKA_PRODUCER = 0,
        RD_KAFKA_CONSUMER = 1
    }

    public enum RdKafkaConfRes
    {
        RD_KAFKA_CONF_UNKNOWN = -2,
        RD_KAFKA_CONF_INVALID = -1,
        RD_KAFKA_CONF_OK = 0
    }

    public enum RdKafkaErrorCode
    {
        LOCAL_BADMSG = -199,
        LOCAL_BADCOMPRESSION = -198,
        LOCAL_DESTROY = -197,
        LOCAL_FAIL = -196,
        LOCAL_TRANSPORT = -195,
        LOCAL_CRITSYSRESOURCE = -194,
        LOCAL_RESOLVE = -193,
        LOCAL_MSGTIMEDOUT = -192,
        LOCAL_PARTITIONEOF = -191,
        LOCAL_UNKNOWNPARTITION = -190,
        LOCAL_FS = -189,
        LOCAL_UNKNOWNTOPIC = -188,
        LOCAL_ALLBROKERSDOWN = -187,
        LOCAL_INVALIDARG = -186,
        LOCAL_TIMEDOUT = -185,
        LOCAL_QUEUEFULL = -184,
        LOCAL_ISRINSUFF = -183,
        LOCAL_NODEUPDATE = -182,
        LOCAL_SSL = -181,
        LOCAL_WAITCOORD = -180,
        LOCAL_UNKNOWNGROUP = -179,
        LOCAL_INPROGRESS = -178,
        LOCAL_PREVINPROGRESS = -177,
        LOCAL_EXISTINGSUBSCRIPTION = -176,
        LOCAL_ASSIGNPARTITIONS=  -175,
        LOCAL_REVOKEPARTITIONS = -174,
        LOCAL_CONFLICT = -173,
        LOCAL_STATE = -172,
        LOCAL_UNKNOWNPROTOCOL = -171,
        LOCAL_NOTIMPLEMENTED = -170,
        LOCAL_AUTHENTICATION = -169,
        LOCAL_NOOFFSET = -168,
        LOCAL_OUTDATED = -167,
        LOCAL_TIMEDOUTQUEUE = -166,
        LOCAL_UNSUPPORTEDFEATURE = -165,
        LOCAL_WAITCACHE = -164,
        LOCAL_INTR = -163,
        LOCAL_KEYSERIALIZATION = -162,
        LOCAL_VALUESERIALIZATION = -161,
        LOCAL_KEYDESERIALIZATION = -160,
        LOCAL_VALUEDESERIALIZATION = -159,
        LOCAL_PARTIAL = -158,
        LOCAL_READONLY = -157,
        LOCAL_NOENT = -156,
        LOCAL_UNDERFLOW = -155,
        LOCAL_INVALIDTYPE = -154,
        LOCAL_RETRY = -153,
        LOCAL_PURGEQUEUE = -152,
        LOCAL_PURGEINFLIGHT = -151,
        LOCAL_FATAL = -150,
        LOCAL_INCONSISTENT = -149,
        LOCAL_GAPLESSGUARANTEE = -148,
        LOCAL_MAXPOLLEXCEEDED = -147,
        UNKNOWN = -1,
        NOERROR = 0,
        OFFSETOUTOFRANGE = 1,
        INVALIDMSG = 2,
        UNKNOWNTOPICORPART = 3,
        INVALIDMSGSIZE = 4,
        LEADERNOTAVAILABLE = 5,
        NOTLEADERFORPARTITION = 6,
        REQUESTTIMEDOUT = 7,
        BROKERNOTAVAILABLE = 8,
        REPLICANOTAVAILABLE = 9,
        MSGSIZETOOLARGE = 10,
        STALECTRLEPOCH = 11,
        OFFSETMETADATATOOLARGE = 12,
        NETWORKEXCEPTION = 13,
        GROUPLOADINPRORESS = 14,
        GROUPCOORDINATORNOTAVAILABLE = 15,
        NOTCOORDINATORFORGROUP = 16,
        TOPICEXCEPTION = 17,
        RECORDLISTTOOLARGE = 18,
        NOTENOUGHREPLICAS = 19,
        NOTENOUGHREPLICASAFTERAPPEND = 20,
        INVALIDREQUIREDACKS = 21,
        ILLEGALGENERATION = 22,
        INCONSISTENTGROUPPROTOCOL = 23,
        INVALIDGROUPID = 24,
        UNKNOWNMEMBERID = 25,
        INVALIDSESSIONTIMEOUT = 26,
        REBALANCEINPROGRESS = 27,
        INVALIDCOMMITOFFSETSIZE = 28,
        TOPICAUTHORIZATIONFAILED = 29,
        GROUPAUTHORIZATIONFAILED = 30,
        CLUSTERAUTHORIZATIONFAILED = 31,
        INVALIDTIMESTAMP = 32,
        UNSUPPORTEDSASLMECHANISM = 33,
        ILLEGALSASLSTATE = 34,
        UNSUPPORTEDVERSION = 35,
        TOPICALREADYEXISTS = 36,
        INVALIDPARTITIONS = 37,
        INVALIDREPLICATIONFACTOR = 38,
        INVALIDREPLICAASSIGNMENT = 39,
        INVALIDCONFIG = 40,
        NOTCONTROLLER = 41,
        INVALIDREQUEST = 42,
        UNSUPPORTEDFORMESSAGEFORMAT = 43,
        POLICYVIOLATION = 44,
        OUTOFORDERSEQUENCENUMBER = 45,
        DUPLICATESEQUENCENUMBER = 46,
        INVALIDPRODUCEREPOCH = 47,
        INVALIDTXNSTATE = 48,
        INVALIDPRODUCERIDMAPPING = 49,
        INVALIDTRANSACTIONTIMEOUT = 50,
        CONCURRENTTRANSACTIONS = 51,
        TRANSACTIONCOORDINATORFENCED = 52,
        TRANSACTIONALIDAUTHORIZATIONFAILED = 53,
        SECURITYDISABLED = 54,
        OPERATIONNOTATTEMPTED = 55,
        KAFKASTORAGEERROR = 56,
        LOGDIRNOTFOUND = 57,
        SASLAUTHENTICATIONFAILED = 58,
        UNKNOWNPRODUCERID = 59,
        REASSIGNMENTINPROGRESS = 60,
        DELEGATIONTOKENAUTHDISABLED = 61,
        DELEGATIONTOKENNOTFOUND = 62,
        DELEGATIONTOKENOWNERMISMATCH = 63,
        DELEGATIONTOKENREQUESTNOTALLOWED = 64,
        DELEGATIONTOKENAUTHORIZATIONFAILED = 65,
        DELEGATIONTOKENEXPIRED = 66,
        INVALIDPRINCIPALTYPE = 67,
        NONEMPTYGROUP = 68,
        GROUPIDNOTFOUND = 69,
        FETCHSESSIONIDNOTFOUND = 70,
        INVALIDFETCHSESSIONEPOCH = 71,
        LISTENERNOTFOUND = 72,
        TOPICDELETIONDISABLED = 73,
        UNSUPPORTEDCOMPRESSIONTYPE = 74
    }
}