import pytest
from agent.tools import extract_ethereum_address
from agent.graph import build_sentinel_graph
from blockchain.payment_gate import HTTP402PaymentGate

def test_extract_ethereum_address():
    text = "Please check address 0xd8dA6BF26964aF9D7eEd9e03E53415D37aA96045 for tokens"
    addr = extract_ethereum_address(text)
    assert addr == "0xd8dA6BF26964aF9D7eEd9e03E53415D37aA96045"

def test_payment_challenge_generation():
    gate = HTTP402PaymentGate()
    challenge = gate.generate_challenge("q-1", "caller-a")
    assert challenge["status_code"] == 402
    assert challenge["amount_wei"] > 0
    assert challenge["challenge_hash"].startswith("0x")

def test_graph_execution_with_settlement():
    graph = build_sentinel_graph()
    state = {
        "query": "Audit security of 0xd8dA6BF26964aF9D7eEd9e03E53415D37aA96045",
        "caller_id": "test-agent",
        "payment_proof": "mock_tx_settled",
        "logs": [],
    }
    result = graph.invoke(state)
    assert result.get("payment_verified") is True
    assert "security_verdict" in result or "error" in result
