import uuid
from typing import Dict, Any
from .state import AgentState
from .tools import extract_ethereum_address, inspect_address_onchain
from blockchain.payment_gate import HTTP402PaymentGate

payment_gate = HTTP402PaymentGate()

def parse_intent_node(state: AgentState) -> Dict[str, Any]:
    """Node: Parses the query to identify targets and task intentions."""
    query = state.get("query", "")
    logs = list(state.get("logs", []))
    logs.append(f"Parsing intent for query: '{query}'")

    target_address = state.get("target_address") or extract_ethereum_address(query)

    network = state.get("network", "mainnet")
    if "sepolia" in query.lower():
        network = "sepolia"
    elif "base" in query.lower():
        network = "base"

    task_type = "inspect"
    if any(k in query.lower() for k in ["audit", "risk", "security", "exploit"]):
        task_type = "risk_audit"

    return {
        "target_address": target_address,
        "network": network,
        "task_type": task_type,
        "logs": logs,
    }

def payment_gate_node(state: AgentState) -> Dict[str, Any]:
    """Node: Evaluates HTTP-402 micro-settlement requirement."""
    logs = list(state.get("logs", []))
    payment_proof = state.get("payment_proof")
    caller_id = state.get("caller_id", "anonymous-agent")

    # If already verified or payment proof provided
    if payment_proof and payment_gate.verify_payment(payment_proof, ""):
        logs.append(f"Payment proof verified successfully for caller: {caller_id}")
        return {
            "payment_verified": True,
            "payment_required": False,
            "logs": logs,
        }

    # If task is an intensive risk audit, require payment challenge
    if state.get("task_type") == "risk_audit" and not state.get("payment_verified"):
        query_id = str(uuid.uuid4())[:8]
        challenge = payment_gate.generate_challenge(query_id, caller_id)
        logs.append(f"HTTP-402 Challenge generated. Micropayment required ({challenge['amount_wei']} wei).")
        return {
            "payment_required": True,
            "payment_verified": False,
            "payment_challenge": challenge,
            "logs": logs,
        }

    logs.append("Standard inspection query: bypassed payment gate.")
    return {
        "payment_required": False,
        "payment_verified": True,
        "logs": logs,
    }

def fetch_onchain_data_node(state: AgentState) -> Dict[str, Any]:
    """Node: Deterministic on-chain data retrieval."""
    logs = list(state.get("logs", []))
    target_address = state.get("target_address")
    network = state.get("network", "mainnet")

    if not target_address:
        logs.append("Error: No target address specified or found.")
        return {
            "error": "No valid Ethereum address identified in request.",
            "logs": logs,
        }

    logs.append(f"Fetching on-chain state for {target_address} on {network}")
    data = inspect_address_onchain(target_address, network=network)
    logs.append(f"Fetched on-chain state: is_contract={data.get('is_contract')}, balance={data.get('balance_eth')} ETH")

    return {
        "onchain_data": data,
        "logs": logs,
    }

def evaluate_security_node(state: AgentState) -> Dict[str, Any]:
    """Node: Analyzes on-chain state for security patterns and risk flags."""
    logs = list(state.get("logs", []))
    data = state.get("onchain_data", {})
    logs.append("Evaluating security heuristics and activity indicators...")

    risk_score = 0
    flags = []

    if data.get("is_contract"):
        bytecode_len = data.get("bytecode_length_bytes", 0)
        if bytecode_len < 50:
            risk_score += 30
            flags.append("Suspiciously small contract bytecode (possible minimal proxy or unverified stub).")
        else:
            flags.append(f"Verified contract bytecode length ({bytecode_len} bytes).")

        if data.get("nonce_tx_count", 0) == 0:
            flags.append("Contract has not initiated outgoing transactions.")
    else:
        flags.append("Externally Owned Account (EOA) wallet.")
        if data.get("nonce_tx_count", 0) == 0:
            risk_score += 20
            flags.append("Fresh account with 0 outgoing transactions.")
        elif data.get("nonce_tx_count", 0) > 1000:
            flags.append("High-activity account (>1000 transactions).")

    verdict_level = "LOW"
    if risk_score >= 50:
        verdict_level = "HIGH"
    elif risk_score >= 20:
        verdict_level = "MEDIUM"

    verdict = {
        "risk_level": verdict_level,
        "risk_score": risk_score,
        "flags": flags,
        "target": data.get("address"),
    }
    logs.append(f"Security analysis complete. Assigned risk: {verdict_level} ({risk_score}/100)")

    return {
        "security_verdict": verdict,
        "logs": logs,
    }

def synthesize_report_node(state: AgentState) -> Dict[str, Any]:
    """Node: Aggregates findings into the structured agent response."""
    logs = list(state.get("logs", []))
    logs.append("Synthesizing final execution report.")

    output = {
        "status": "COMPLETED",
        "query": state.get("query"),
        "target_address": state.get("target_address"),
        "network": state.get("network"),
        "task_type": state.get("task_type"),
        "onchain_overview": state.get("onchain_data"),
        "security_analysis": state.get("security_verdict"),
        "execution_trace": logs,
    }

    return {
        "final_output": output,
        "logs": logs,
    }
