using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HelloInt64
{
    public struct Long
    {
        private double low;
        private double high;

        internal Long(double low, double high)
        {
            this.low = low;
            this.high = high;
        }

        internal Long(double[] lowHighPair)
        {
            this.low = lowHighPair[0];
            this.high = lowHighPair[1];
        }

        public static implicit operator Long(int value)
        {
            if (value >= 0)
            {
                return new Long(value, 0d);
            }
            else
            {
                return new Long(value + TWO_PWR_32_DBL, -TWO_PWR_32_DBL);
            }
        }

        public static implicit operator int(Long value)
        {
            return LowBits(value);
        }

        public static implicit operator Long(Int64 value)
        {
            return MakeFromBits((int)(value >> 32), (int)value);
        }

        public static explicit operator Long(double value)
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

        private static Long MakeFromBits(int lowBits, int highBits)
        {
            double high = highBits * TWO_PWR_32_DBL;
            double low = lowBits;
            if (lowBits < 0)
            {
                low += TWO_PWR_32_DBL;
            }
            return new Long(low, high);
        }

        /*
         * Make a new instance equal to valueLow+valueHigh. The arguments do not need
         * to be normalized, though by convention valueHigh and valueLow will hold the
         * high and low bits, respectively.
         */
        private static Long create(double valueLow, double valueHigh)
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

            return new Long(valueLow, valueHigh);
        }

        /**
         * Number of bits we expect to be accurate for a double representing a large
         * integer.
         */
        private static readonly int PRECISION_BITS = 48;

        private static readonly Long TWO_PWR_24 = (0x1000000L);

        private static readonly double LOW_MAX = 4294967295d;
        private static readonly double LOW_MIN = 0;

        private static readonly double HIGH_MAX = 9223372032559808512d;
        private static readonly double HIGH_MIN = -9223372036854775808d;

        private static readonly double TWO_PWR_15_DBL = 0x8000;
        private static readonly double TWO_PWR_16_DBL = 0x10000;
        private static readonly double TWO_PWR_31_DBL = TWO_PWR_16_DBL * TWO_PWR_15_DBL;
        private static readonly double TWO_PWR_32_DBL = TWO_PWR_16_DBL * TWO_PWR_16_DBL;
        private static readonly double TWO_PWR_48_DBL = TWO_PWR_32_DBL * TWO_PWR_16_DBL;
        private static readonly double TWO_PWR_63_DBL = TWO_PWR_32_DBL * TWO_PWR_31_DBL;
        private static readonly double TWO_PWR_64_DBL = TWO_PWR_32_DBL * TWO_PWR_32_DBL;

        private static readonly double LN_2 = Math.Log(2);
        private static readonly Long MAX_VALUE = Int64.MaxValue;
        private static readonly Long MIN_VALUE = Int64.MinValue;
        private static readonly Long NEG_ONE = -1;
        private static readonly Long ZERO = 0;
        private static readonly Long ONE = 1;
        private static readonly Long TWO = 2;

        public static Long operator +(Long left, Long right)
        {
            double newHigh = left.high + right.high;
            double newLow = left.low + right.low;
            return new Long(newLow, newHigh);
        }

        public static Long operator *(Long a, Long b)
        {
            if (IsZero(a))
            {
                return ZERO;
            }
            if (IsZero(b))
            {
                return ZERO;
            }

            // handle MIN_VALUE carefully, because neg(MIN_VALUE)==MIN_VALUE
            if (Equals(a, MIN_VALUE))
            {
                return multByMinValue(b);
            }
            if (Equals(b, MIN_VALUE))
            {
                return multByMinValue(a);
            }

            // If either argument is negative, change it to positive, multiply,
            // and then negate the result.
            if (IsNegative(a))
            {
                if (IsNegative(b))
                {
                    return (-a) * (-b);
                }
                else
                {
                    return -((-a) * b);
                }
            }
            Assert(!IsNegative(a));
            if (IsNegative(b))
            {
                return -(a * (-b));
            }
            Assert(!IsNegative(b));

            // If both numbers are small, use float multiplication
            if ((a < TWO_PWR_24) && (b < TWO_PWR_24))
            {
                return create(((double)a) * ((double)b), 0.0);
            }

            // Divide each number into 4 chunks of 16 bits, and then add
            // up 4x4 multiplies. Skip the six multiplies where the result
            // mod 2^64 would be 0.
            double a3 = a.high % TWO_PWR_48_DBL;
            double a4 = a.high - a3;
            double a1 = a.low % TWO_PWR_16_DBL;
            double a2 = a.low - a1;

            double b3 = b.high % TWO_PWR_48_DBL;
            double b4 = b.high - b3;
            double b1 = b.low % TWO_PWR_16_DBL;
            double b2 = b.low - b1;

            Long res = ZERO;

            res = AddTimes(res, a4, b1);
            res = AddTimes(res, a3, b2);
            res = AddTimes(res, a3, b1);
            res = AddTimes(res, a2, b3);
            res = AddTimes(res, a2, b2);
            res = AddTimes(res, a2, b1);
            res = AddTimes(res, a1, b4);
            res = AddTimes(res, a1, b3);
            res = AddTimes(res, a1, b2);
            res = AddTimes(res, a1, b1);

            return res;
        }

        private static Long AddTimes(Long accum, double a, double b)
        {
            if (a == 0.0)
            {
                return accum;
            }
            if (b == 0.0)
            {
                return accum;
            }
            return (accum + create(a * b, 0.0));
        }

        private static Long multByMinValue(Long value)
        {
            if (IsOdd(value))
            {
                return MIN_VALUE;
            }
            else
            {
                return ZERO;
            }
        }

        public static Long operator /(Long a, Long b)
        {
            if (IsZero(b))
            {
                throw new DivideByZeroException();
            }

            if (IsZero(a))
            {
                return ZERO;
            }

            if (a.Equals(MIN_VALUE))
            {
                // handle a==MIN_VALUE carefully because of overflow issues
                if (b.Equals(ONE) || b.Equals(NEG_ONE))
                {
                    // this strange exception is described in JLS3 17.17.2
                    return MIN_VALUE;
                }
                // at this point, abs(b) >= 2, so |a/b| < -MIN_VALUE
                Long halfa = ShiftRight(a, 1);
                Long approx = ShiftLeft((halfa / b), 1);
                Long rem = (a - (b * approx));
                Assert((rem > MIN_VALUE));
                return (approx + (rem / b));
            }

            if (Equals(b, MIN_VALUE))
            {
                Assert(!Equals(a, MIN_VALUE));
                return ZERO;
            }

            // To keep the implementation compact, make a and be
            // both be positive and swap the sign of the result
            // if necessary.
            if (IsNegative(a))
            {
                if (IsNegative(b))
                {
                    return ((-a) / (-b));
                }
                else
                {
                    return (-((-a) / b));
                }
            }
            Assert(!IsNegative(a));
            if (IsNegative(b))
            {
                return (-(a / (-b)));
            }
            Assert(!IsNegative(b));

            // Use float division to approximate the answer.
            // Repeat until the remainder is less than b.
            Long result = ZERO;
            Long remainder = a;
            while (remainder > b)
            {
                // approximate using float division
                Long deltaResult = (Long)(Math.Floor(ToDoubleRoundDown(remainder)
                    / ToDoubleRoundUp(b)));
                if (IsZero(deltaResult))
                {
                    deltaResult = ONE;
                }
                Long deltaRem = (deltaResult * b);

                //Assert(deltaResult > ONE);
                //Assert(deltaRem < remainder);
                result = result + deltaResult;
                remainder = remainder - deltaRem;
            }

            return result;
        }

        public static Long operator -(Long a)
        {
            if (Equals(a, MIN_VALUE))
            {
                return MIN_VALUE;
            }
            double newHigh = -a.high;
            double newLow = -a.low;
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
            return new Long(newLow, newHigh);
        }

        public static Long operator -(Long left, Long right)
        {
            double newHigh = left.high - right.high;
            double newLow = left.low - right.low;
            return create(newLow, newHigh);
        }

        public override string ToString()
        {
            if (IsZero(this))
            {
                return "0";
            }

            if (Equals(this, MIN_VALUE))
            {
                // Special-case MIN_VALUE because neg(MIN_VALUE)==MIN_VALUE
                return "-9223372036854775808";
            }

            if (IsNegative(this))
            {
                return "-" + (-this).ToString();
            }

            Long rem = this;
            String res = "";

            while (!IsZero(rem))
            {
                // Do several digits each time through the loop, so as to
                // minimize the calls to the very expensive emulated div.
                int tenPowerZeroes = 9;
                int tenPower = 1000000000;
                Long tenPowerLong = tenPower;

                Long remDivTenPower = (rem / tenPowerLong);
                String digits = "" + (int)((rem - (remDivTenPower * tenPowerLong)));
                rem = remDivTenPower;

                if (!IsZero(rem))
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

        public bool Equals(Long other)
        {
            return Equals(this, other);
        }

        public static bool Equals(Long left, Long right)
        {
            return ((left.low == right.low) && (left.high == right.high));
        }

        private static bool IsNegative(Long value)
        {
            return value.high < 0;
        }

        private static bool IsZero(Long value)
        {
            return value.low == 0.0 && value.high == 0.0;
        }

        private static bool IsOdd(Long value)
        {
            return (LowBits(value) & 1) == 1;
        }

        private static double ToDoubleRoundDown(Long a)
        {
            int magnitute = (int)(Math.Log(a.high) / LN_2);
            if (magnitute <= PRECISION_BITS)
            {
                return (double)a;
            }
            else
            {
                int diff = magnitute - PRECISION_BITS;
                int toSubtract = (1 << diff) - 1;
                return a.high + (a.low - toSubtract);
            }
        }

        private static double ToDoubleRoundUp(Long a)
        {
            int magnitute = (int)(Math.Log(a.high) / LN_2);
            if (magnitute <= PRECISION_BITS)
            {
                return (double)a;
            }
            else
            {
                int diff = magnitute - PRECISION_BITS;
                int toAdd = (1 << diff) - 1;
                return a.high + (a.low + toAdd);
            }
        }

        private static int LowBits(Long value)
        {
            if (value.low >= TWO_PWR_31_DBL)
            {
                return (int)(value.low - TWO_PWR_32_DBL);
            }
            else
            {
                return (int)value.low;
            }
        }

        public static Long ShiftLeft(Long a, int n)
        {
            n &= 63;

            if (Equals(a, MIN_VALUE))
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

            if (IsNegative(a))
            {
                return (-ShiftLeft((-a), n));
            }

            double twoToN = Pow(n);

            double newHigh = a.high * twoToN % TWO_PWR_64_DBL;
            double newLow = a.low * twoToN;
            double diff = newLow - (newLow % TWO_PWR_32_DBL);
            newHigh += diff;
            newLow -= diff;
            if (newHigh >= TWO_PWR_63_DBL)
            {
                newHigh -= TWO_PWR_64_DBL;
            }

            return new Long(newLow, newHigh);
        }

        public static Long ShiftRight(Long a, int n)
        {
            n &= 63;
            double shiftFact = Pow(n);
            double newHigh = Math.Floor(a.high / shiftFact);
            double newLow = Math.Floor(a.low / shiftFact);

            /*
             * Doing the above floors separately on each component is safe. If n<32,
             * a.high/shiftFact is guaranteed to be an integer already. For n>32,
             * a.high/shiftFact will have fractional bits, but we need to discard them
             * as they shift away. We will end up discarding all of a.low in this case,
             * as it divides out to entirely fractional.
             */

            return create(newLow, newHigh);
        }

        /**
         * Logical right shift. It does not preserve the sign of the input.
         */
        private static Long shru(Long a, int n)
        {
            n &= 63;
            Long sr = ShiftRight(a, n);
            if (IsNegative(a))
            {
                // the following changes the high bits to 0, using
                // a formula from JLS3 section 15.19
                sr = sr + ShiftLeft(TWO, 63 - n);
            }

            return sr;
        }

        /**
         * Return a power of two as a double.
         * 
         * @return 2 raised to the <code>n</code>
         */
        private static double Pow(int n)
        {
            if (n <= 30)
            {
                return (1 << n);
            }
            else
            {
                return Pow(30) * Pow(n - 30);
            }
        }

        [ConditionalAttribute("DEBUG")]
        private static void Assert(bool condition)
        {
            if (!condition) throw new Exception("Assertion failed");
        }
    }
}
