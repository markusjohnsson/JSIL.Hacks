using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HelloInt64
{

    /**
     * Implements a Java <code>long</code> in a way that can be translated to
     * JavaScript.
     */
    public class LongLib
    {
        /*
         * Implementation: An array of two doubles, low and high, such that high+low
         * is mathematically equivalent to the original integer. Since a JavaScript
         * Number does not hold enough bits to precisely calculate high+low, all
         * operations must be implemented carefully. "low" is always between 0 and
         * 2^32-1 inclusive. "high" is always between -2^63 and 2^63-2^32 inclusive
         * and is a multiple of 2^32. The sign of the number is determined entirely by
         * "high". Since low is always positive, small negative numbers are encoded
         * with high=-2^32. For example, -1 is encoded as { high=-2^32, low=2^32-1 }.
         * 
         * Note that this class must be careful using type "long". Being the
         * implementation of the long type for web mode, any place it uses a long is
         * not usable in web mode. There are currently two such methods:
         * {@link #toLong(double[])} and {@link #make(long)}.
         * 
         * The GWT RPC serialization code is dependent on the internal format of the
         * long type; any changes made to this class should be reflected in the
         * implementations of SerializationStreamReader and Writer.
         */

        // Between -128 and 127.
        static double[][] cachedInts = new double[256][];


        static readonly double LN_2 = Math.Log(2);
        static readonly double[] MAX_VALUE = typeChange(Int64.MaxValue);
        static readonly double[] MIN_VALUE = typeChange(Int64.MinValue);
        static readonly double[] NEG_ONE = fromInt(-1);
        static readonly double[] ONE = fromInt(1);
        static readonly double[] TWO = fromInt(2);

        /**
         * Half of the number of bits we expect to be precise.
         * 
         * @see LongLib#PRECISION_BITS
         */
        static readonly double[] TWO_PWR_24 = typeChange(0x1000000L);

        static readonly double[] ZERO = fromInt(0);


        /**
         * Set this to false before calling any methods when using this class outside
         * of GWT!
         */
        //public static bool RUN_IN_JVM = true;
        
        /**
         * Number of bits we expect to be accurate for a double representing a large
         * integer.
         */
        private static readonly int PRECISION_BITS = 48;

        /**
         * Index of the high bits in a 2-double array.
         */
        private static readonly int HIGH = 1;
        private static readonly double HIGH_MAX = 9223372032559808512d;
        private static readonly double HIGH_MIN = -9223372036854775808d;

        /**
         * Index of the low bits in a 2-double array.
         */
        private static readonly int LOW = 0;
        private static readonly double LOW_MAX = 4294967295d;
        private static readonly double LOW_MIN = 0;

        private static readonly double TWO_PWR_15_DBL = 0x8000;
        private static readonly double TWO_PWR_16_DBL = 0x10000;
        private static readonly double TWO_PWR_31_DBL = TWO_PWR_16_DBL * TWO_PWR_15_DBL;
        private static readonly double TWO_PWR_32_DBL = TWO_PWR_16_DBL * TWO_PWR_16_DBL;
        private static readonly double TWO_PWR_48_DBL = TWO_PWR_32_DBL * TWO_PWR_16_DBL;
        private static readonly double TWO_PWR_63_DBL = TWO_PWR_32_DBL * TWO_PWR_31_DBL;
        private static readonly double TWO_PWR_64_DBL = TWO_PWR_32_DBL * TWO_PWR_32_DBL;

        public static double[] add(double[] a, double[] b)
        {
            double newHigh = a[HIGH] + b[HIGH];
            double newLow = a[LOW] + b[LOW];
            return create(newLow, newHigh);
        }

        public static double[] and(double[] a, double[] b)
        {
            return makeFromBits(highBits(a) & highBits(b), lowBits(a) & lowBits(b));
        }

        /**
         * Compare the receiver to the argument.
         * 
         * @return 0 if they are the same, 1 if the receiver is greater, -1 if the
         *         argument is greater.
         */
        public static int compare(double[] a, double[] b)
        {
            if (eq(a, b))
            {
                return 0;
            }

            bool nega = isNegative(a);
            bool negb = isNegative(b);
            if (nega && !negb)
            {
                return -1;
            }
            if (!nega && negb)
            {
                return 1;
            }

            // at this point, the signs are the same, so subtraction will not overflow
            Assert(nega == negb);
            if (isNegative(sub(a, b)))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public static double[] div(double[] a, double[] b)
        {
            if (isZero(b))
            {
                throw new ArithmeticException("/ by zero");
            }
            if (isZero(a))
            {
                return ZERO;
            }

            if (eq(a, MIN_VALUE))
            {
                // handle a==MIN_VALUE carefully because of overflow issues
                if (eq(b, ONE) || eq(b, NEG_ONE))
                {
                    // this strange exception is described in JLS3 17.17.2
                    return MIN_VALUE;
                }
                // at this point, abs(b) >= 2, so |a/b| < -MIN_VALUE
                double[] halfa = shr(a, 1);
                double[] approx = shl(div(halfa, b), 1);
                double[] rem = sub(a, mul(b, approx));
                Assert(gt(rem, MIN_VALUE));
                return add(approx, div(rem, b));
            }

            if (eq(b, MIN_VALUE))
            {
                Assert(!eq(a, MIN_VALUE));
                return ZERO;
            }

            // To keep the implementation compact, make a and be
            // both be positive and swap the sign of the result
            // if necessary.
            if (isNegative(a))
            {
                if (isNegative(b))
                {
                    return div(neg(a), neg(b));
                }
                else
                {
                    return neg(div(neg(a), b));
                }
            }
            Assert(!isNegative(a));
            if (isNegative(b))
            {
                return neg(div(a, neg(b)));
            }
            Assert(!isNegative(b));

            // Use float division to approximate the answer.
            // Repeat until the remainder is less than b.
            double[] result = ZERO;
            double[] remainder = a;
            while (gte(remainder, b))
            {
                // approximate using float division
                double[] deltaResult = fromDouble(Math.Floor(toDoubleRoundDown(remainder)
                    / toDoubleRoundUp(b)));
                if (isZero(deltaResult))
                {
                    deltaResult = ONE;
                }
                double[] deltaRem = mul(deltaResult, b);

                Assert(gte(deltaResult, ONE));
                Assert(lte(deltaRem, remainder));
                result = add(result, deltaResult);
                remainder = sub(remainder, deltaRem);
            }

            return result;
        }

        public static bool eq(double[] a, double[] b)
        {
            return ((a[LOW] == b[LOW]) && (a[HIGH] == b[HIGH]));
        }

        public static double[] fromDouble(double value)
        {
            if (Double.IsNaN(value))
            {
                return ZERO;
            }
            if (value < -TWO_PWR_63_DBL)
            {
                return MIN_VALUE;
            }
            if (value >= TWO_PWR_63_DBL)
            {
                return MAX_VALUE;
            }
            if (value > 0)
            {
                return create(Math.Floor(value), 0.0);
            }
            else
            {
                return create(Math.Ceiling(value), 0.0);
            }
        }

        public static double[] fromInt(int value)
        {
            if (value > -129 && value < 128)
            {
                int rebase = value + 128;
                double[] result = cachedInts[rebase];
                if (result == null)
                {
                    result = cachedInts[rebase] = internalFromInt(value);
                }
                return result;
            }
            return internalFromInt(value);
        }

        public static bool gt(double[] a, double[] b)
        {
            return compare(a, b) > 0;
        }

        public static bool gte(double[] a, double[] b)
        {
            return compare(a, b) >= 0;
        }

        public static bool lt(double[] a, double[] b)
        {
            return compare(a, b) < 0;
        }

        public static bool lte(double[] a, double[] b)
        {
            return compare(a, b) <= 0;
        }

        public static double[] mod(double[] a, double[] b)
        {
            return sub(a, mul(div(a, b), b));
        }

        public static double[] mul(double[] a, double[] b)
        {
            if (isZero(a))
            {
                return ZERO;
            }
            if (isZero(b))
            {
                return ZERO;
            }

            // handle MIN_VALUE carefully, because neg(MIN_VALUE)==MIN_VALUE
            if (eq(a, MIN_VALUE))
            {
                return multByMinValue(b);
            }
            if (eq(b, MIN_VALUE))
            {
                return multByMinValue(a);
            }

            // If either argument is negative, change it to positive, multiply,
            // and then negate the result.
            if (isNegative(a))
            {
                if (isNegative(b))
                {
                    return mul(neg(a), neg(b));
                }
                else
                {
                    return neg(mul(neg(a), b));
                }
            }
            Assert(!isNegative(a));
            if (isNegative(b))
            {
                return neg(mul(a, neg(b)));
            }
            Assert(!isNegative(b));

            // If both numbers are small, use float multiplication
            if (lt(a, TWO_PWR_24) && lt(b, TWO_PWR_24))
            {
                return create(toDouble(a) * toDouble(b), 0.0);
            }

            // Divide each number into 4 chunks of 16 bits, and then add
            // up 4x4 multiplies. Skip the six multiplies where the result
            // mod 2^64 would be 0.
            double a3 = a[HIGH] % TWO_PWR_48_DBL;
            double a4 = a[HIGH] - a3;
            double a1 = a[LOW] % TWO_PWR_16_DBL;
            double a2 = a[LOW] - a1;

            double b3 = b[HIGH] % TWO_PWR_48_DBL;
            double b4 = b[HIGH] - b3;
            double b1 = b[LOW] % TWO_PWR_16_DBL;
            double b2 = b[LOW] - b1;

            double[] res = ZERO;

            res = addTimes(res, a4, b1);
            res = addTimes(res, a3, b2);
            res = addTimes(res, a3, b1);
            res = addTimes(res, a2, b3);
            res = addTimes(res, a2, b2);
            res = addTimes(res, a2, b1);
            res = addTimes(res, a1, b4);
            res = addTimes(res, a1, b3);
            res = addTimes(res, a1, b2);
            res = addTimes(res, a1, b1);

            return res;
        }

        public static double[] neg(double[] a)
        {
            if (eq(a, MIN_VALUE))
            {
                return MIN_VALUE;
            }
            double newHigh = -a[HIGH];
            double newLow = -a[LOW];
            if (newLow > LOW_MAX)
            {
                newLow -= TWO_PWR_32_DBL;
                newHigh += TWO_PWR_32_DBL;
            }
            if (newLow < LOW_MIN)
            {
                newLow += TWO_PWR_32_DBL;
                newHigh -= TWO_PWR_32_DBL;
            }
            return createNormalized(newLow, newHigh);
        }

        public static bool neq(double[] a, double[] b)
        {
            return ((a[LOW] != b[LOW]) || (a[HIGH] != b[HIGH]));
        }

        public static double[] not(double[] a)
        {
            return makeFromBits(~highBits(a), ~lowBits(a));
        }

        public static double[] or(double[] a, double[] b)
        {
            return makeFromBits(highBits(a) | highBits(b), lowBits(a) | lowBits(b));
        }

        public static double[] shl(double[] a, int n)
        {
            n &= 63;

            if (eq(a, MIN_VALUE))
            {
                if (n == 0)
                {
                    return a;
                }
                else
                {
                    return ZERO;
                }
            }

            if (isNegative(a))
            {
                return neg(shl(neg(a), n));
            }

            double twoToN = pwrAsDouble(n);

            double newHigh = a[HIGH] * twoToN % TWO_PWR_64_DBL;
            double newLow = a[LOW] * twoToN;
            double diff = newLow - (newLow % TWO_PWR_32_DBL);
            newHigh += diff;
            newLow -= diff;
            if (newHigh >= TWO_PWR_63_DBL)
            {
                newHigh -= TWO_PWR_64_DBL;
            }

            return createNormalized(newLow, newHigh);
        }

        public static double[] shr(double[] a, int n)
        {
            n &= 63;
            double shiftFact = pwrAsDouble(n);
            double newHigh = Math.Floor(a[HIGH] / shiftFact);
            double newLow = Math.Floor(a[LOW] / shiftFact);

            /*
             * Doing the above floors separately on each component is safe. If n<32,
             * a[HIGH]/shiftFact is guaranteed to be an integer already. For n>32,
             * a[HIGH]/shiftFact will have fractional bits, but we need to discard them
             * as they shift away. We will end up discarding all of a[LOW] in this case,
             * as it divides out to entirely fractional.
             */

            return create(newLow, newHigh);
        }

        /**
         * Logical right shift. It does not preserve the sign of the input.
         */
        public static double[] shru(double[] a, int n)
        {
            n &= 63;
            double[] sr = shr(a, n);
            if (isNegative(a))
            {
                // the following changes the high bits to 0, using
                // a formula from JLS3 section 15.19
                sr = add(sr, shl(TWO, 63 - n));
            }

            return sr;
        }

        public static double[] sub(double[] a, double[] b)
        {
            double newHigh = a[HIGH] - b[HIGH];
            double newLow = a[LOW] - b[LOW];
            return create(newLow, newHigh);
        }

        /**
         * Cast from long to double or float.
         */
        public static double toDouble(double[] a)
        {
            return a[HIGH] + a[LOW];
        }

        /**
         * Cast from long to int.
         */
        public static int toInt(double[] a)
        {
            return lowBits(a);
        }

        /**
         * Implicit conversion from long to String.
         */
        public static String toString(double[] a)
        {
            if (isZero(a))
            {
                return "0";
            }

            if (eq(a, MIN_VALUE))
            {
                // Special-case MIN_VALUE because neg(MIN_VALUE)==MIN_VALUE
                return "-9223372036854775808";
            }

            if (isNegative(a))
            {
                return "-" + toString(neg(a));
            }

            double[] rem = a;
            String res = "";

            while (!isZero(rem))
            {
                // Do several digits each time through the loop, so as to
                // minimize the calls to the very expensive emulated div.
                int tenPowerZeroes = 9;
                int tenPower = 1000000000;
                double[] tenPowerLong = fromInt(tenPower);

                double[] remDivTenPower = div(rem, tenPowerLong);
                String digits = "" + toInt(sub(rem, mul(remDivTenPower, tenPowerLong)));
                rem = remDivTenPower;

                if (!isZero(rem))
                {
                    int zeroesNeeded = tenPowerZeroes - digits.Length;
                    for (; zeroesNeeded > 0; zeroesNeeded--)
                    {
                        digits = "0" + digits;
                    }
                }

                res = digits + res;
            }

            return res;
        }

        public static double[] typeChange(long value)
        {
            //if (RUN_IN_JVM)
            {
                return makeFromBits((int)(value >> 32), (int)value);
            }
            //else
            //{
            //    return (double[])(object)(value);
            //}
        }

        public static double[] xor(double[] a, double[] b)
        {
            return makeFromBits(highBits(a) ^ highBits(b), lowBits(a) ^ lowBits(b));
        }

        static long toLong(double[] a)
        {
            return (long)a[HIGH] + (long)a[LOW];
        }

        private static double[] addTimes(double[] accum, double a, double b)
        {
            if (a == 0.0)
            {
                return accum;
            }
            if (b == 0.0)
            {
                return accum;
            }
            return add(accum, create(a * b, 0.0));
        }

        /*
         * Make a new instance equal to valueLow+valueHigh. The arguments do not need
         * to be normalized, though by convention valueHigh and valueLow will hold the
         * high and low bits, respectively.
         */
        private static double[] create(double valueLow, double valueHigh)
        {
            Assert(!Double.IsNaN(valueHigh));
            Assert(!Double.IsNaN(valueLow));
            //Assert(!Double.IsInfinity(valueHigh));
            //Assert(!Double.IsInfinity(valueLow));
            Assert(Math.Floor(valueHigh) == valueHigh);
            Assert(Math.Floor(valueLow) == valueLow);

            // remove overly high bits
            valueHigh %= TWO_PWR_64_DBL;
            valueLow %= TWO_PWR_64_DBL;

            // segregate high and low bits between high and low
            {
                double diffHigh = valueHigh % TWO_PWR_32_DBL;
                double diffLow = Math.Floor(valueLow / TWO_PWR_32_DBL) * TWO_PWR_32_DBL;

                valueHigh = (valueHigh - diffHigh) + diffLow;
                valueLow = (valueLow - diffLow) + diffHigh;
            }

            // Most or all of the while's in this implementation could probably be if's,
            // but they are left as while's for now pending a careful review.

            // make valueLow be positive
            while (valueLow < LOW_MIN)
            {
                valueLow += TWO_PWR_32_DBL;
                valueHigh -= TWO_PWR_32_DBL;
            }

            // make valueLow not too large
            while (valueLow > LOW_MAX)
            {
                valueLow -= TWO_PWR_32_DBL;
                valueHigh += TWO_PWR_32_DBL;
            }

            // make valueHigh within range
            valueHigh = valueHigh % TWO_PWR_64_DBL;
            while (valueHigh > HIGH_MAX)
            {
                valueHigh -= TWO_PWR_64_DBL;
            }
            while (valueHigh < HIGH_MIN)
            {
                valueHigh += TWO_PWR_64_DBL;
            }

            return createNormalized(valueLow, valueHigh);
        }

        /**
         * Create an instance. The high and low parts must be normalized. Normal
         * callers should use the factory method {@link #make(double, double) make}.
         */
        private static double[] createNormalized(double valueLow, double valueHigh)
        {
            Assert(valueHigh <= HIGH_MAX);
            Assert(valueHigh >= HIGH_MIN);
            Assert(valueLow >= 0);
            Assert(valueLow <= LOW_MAX);
            Assert(valueHigh % TWO_PWR_32_DBL == 0);
            Assert(Math.Floor(valueLow) == valueLow); // no fractional bits allowed

            //if (RUN_IN_JVM)
            {
                return new double[] { valueLow, valueHigh };
            }
            //else
            //{
            //    return newLong0(valueLow, valueHigh);
            //}
        }

        private static void Assert(bool assert)
        {
            return;
        }

        private static int highBits(double[] a)
        {
            return (int)(a[HIGH] / TWO_PWR_32_DBL);
        }

        private static double[] internalFromInt(int value)
        {
            if (value >= 0)
            {
                return createNormalized(value, 0.0);
            }
            else
            {
                return createNormalized(value + TWO_PWR_32_DBL, -TWO_PWR_32_DBL);
            }
        }

        private static bool isNegative(double[] a)
        {
            return a[HIGH] < 0;
        }

        private static bool isOdd(double[] a)
        {
            return (lowBits(a) & 1) == 1;
        }

        private static bool isZero(double[] a)
        {
            return a[LOW] == 0.0 && a[HIGH] == 0.0;
        }

        private static int lowBits(double[] a)
        {
            if (a[LOW] >= TWO_PWR_31_DBL)
            {
                return (int)(a[LOW] - TWO_PWR_32_DBL);
            }
            else
            {
                return (int)a[LOW];
            }
        }

        /**
         * Make an instance equivalent to stringing highBits next to lowBits, where
         * highBits and lowBits are assumed to be in 32-bit twos-complement notation.
         * As a result, for any double[] l, the following identity holds:
         * 
         * <blockquote> <code>l == makeFromBits(l.highBits(), l.lowBits())</code>
         * </blockquote>
         */
        private static double[] makeFromBits(int highBits, int lowBits)
        {
            double high = highBits * TWO_PWR_32_DBL;
            double low = lowBits;
            if (lowBits < 0)
            {
                low += TWO_PWR_32_DBL;
            }
            return createNormalized(low, high);
        }

        private static double[] multByMinValue(double[] a)
        {
            if (isOdd(a))
            {
                return MIN_VALUE;
            }
            else
            {
                return ZERO;
            }
        }

        /**
         * Faster web mode implementation doesn't need full type semantics.
         */
        private static double[] newLong0(double valueLow, double valueHigh) {
            return new [] { valueLow, valueHigh };
        }

        /**
         * Return a power of two as a double.
         * 
         * @return 2 raised to the <code>n</code>
         */
        private static double pwrAsDouble(int n)
        {
            if (n <= 30)
            {
                return (1 << n);
            }
            else
            {
                return pwrAsDouble(30) * pwrAsDouble(n - 30);
            }
        }

        private static double toDoubleRoundDown(double[] a)
        {
            int magnitute = (int)(Math.Log(a[HIGH]) / LN_2);
            if (magnitute <= PRECISION_BITS)
            {
                return toDouble(a);
            }
            else
            {
                int diff = magnitute - PRECISION_BITS;
                int toSubtract = (1 << diff) - 1;
                return a[HIGH] + (a[LOW] - toSubtract);
            }
        }

        private static double toDoubleRoundUp(double[] a)
        {
            int magnitute = (int)(Math.Log(a[HIGH]) / LN_2);
            if (magnitute <= PRECISION_BITS)
            {
                return toDouble(a);
            }
            else
            {
                int diff = magnitute - PRECISION_BITS;
                int toAdd = (1 << diff) - 1;
                return a[HIGH] + (a[LOW] + toAdd);
            }
        }


    }
}
