#if NET7_0_OR_GREATER
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace MobileDeviceSharp.AFC.Native
{
    [CustomMarshaller(typeof(string), MarshalMode.Default, typeof(UTF8StringDecomposedMarshaller))]
    [CustomMarshaller(typeof(string), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToUnmanagedIn))]
    public static unsafe class UTF8StringDecomposedMarshaller
    {
        public static byte* ConvertToUnmanaged(string? managed)
        {
            var normalized = managed?.Normalize(NormalizationForm.FormD);
            return Utf8StringMarshaller.ConvertToUnmanaged(normalized);
        }

        public static string? ConvertToManaged(byte* unmanaged)
        {
            var managed = Utf8StringMarshaller.ConvertToManaged(unmanaged);
            return managed?.Normalize(NormalizationForm.FormC);
        }
        public static void Free(byte* unmanaged)
        {
            Utf8StringMarshaller.Free(unmanaged);
        }

        /// <summary>
        /// Custom marshaller to marshal a managed string in canonical form as a decomposed UTF-8 unmanaged string.
        /// </summary>
        public ref struct ManagedToUnmanagedIn
        {
            private readonly Utf8StringMarshaller.ManagedToUnmanagedIn _internalMarshaller;

            public static int BufferSize => Utf8StringMarshaller.ManagedToUnmanagedIn.BufferSize;

            public ManagedToUnmanagedIn()
            {
                _internalMarshaller = new();
            }

            /// <summary>
            /// Initializes the marshaller with a managed string and requested buffer.
            /// </summary>
            /// <param name="managed">The managed string with which to initialize the marshaller.</param>
            /// <param name="buffer">The request buffer whose size is at least <see cref="BufferSize"/>.</param>
            public void FromManaged(string? managed, Span<byte> buffer)
            {
                _internalMarshaller.FromManaged(managed?.Normalize(NormalizationForm.FormD), buffer);
            }


            /// <summary>
            /// Converts the current managed string to an unmanaged string.
            /// </summary>
            /// <returns>An unmanaged string.</returns>
            public byte* ToUnmanaged()
            {
                return _internalMarshaller.ToUnmanaged();
            }

            /// <summary>
            /// Frees any allocated unmanaged memory.
            /// </summary>
            public void Free()
            {
                _internalMarshaller.Free();
            }
        }
    }
}
#endif
