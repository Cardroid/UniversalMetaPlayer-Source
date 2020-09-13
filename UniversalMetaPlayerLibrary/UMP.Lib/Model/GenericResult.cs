using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Lib.Model
{
  /// <summary>
  /// 비동기 함수에서 out 매개 변수 한정자를 사용하지 못하므로 생성되었습니다. 
  /// </summary>
  /// <typeparam name="T">boolean 타입과 함께 쓰일 제네릭 자료형</typeparam>
  public struct GenericResult<T>
  {
    public GenericResult(bool success)
    {
      this.Success = success;
      this.Result = default;
    }
    public GenericResult(bool success, T result)
    {
      this.Success = success;
      this.Result = result;
    }

    public static implicit operator bool(GenericResult<T> d) => d.Success;

    /// <summary>
    /// 성공여부
    /// </summary>
    public bool Success { get; }
    /// <summary>
    /// 작업 결과
    /// </summary>
    public T Result { get; }
  }
}
