import llama_cpp
from llama_cpp import Llama


def llamaCppPython(modelPath, systemContent, userContent):
    # GPUレイヤー数を増やして、GPUの利用を最大化
    model = Llama(
        model_path=modelPath,
        chat_format="llama-2",  # より一般的なフォーマット
        n_ctx=2048,            # コンテキストサイズを増やす
        n_gpu_layers=35,       # GPUレイヤー数を増やす（推奨値は30以上）
        n_batch=512,           # バッチサイズを設定
        verbose=True,          # デバッグ情報の出力を有効化
    )

    try:
        response = model.create_chat_completion(
            messages=[
                {"role": "system", "content": systemContent},
                {"role": "user", "content": userContent},
            ],
            max_tokens=2048,    # 出力トークン数を増やす
            temperature=0.7,     # 応答の多様性を調整
            top_p=0.9,          # 出力の品質を制御
            repeat_penalty=1.1   # 繰り返しを防ぐ
        )

        resultText = response["choices"][0]["message"]["content"]
        return resultText
    
    except Exception as e:
        print(f"エラーが発生しました: {str(e)}")
        return f"エラー: {str(e)}"

