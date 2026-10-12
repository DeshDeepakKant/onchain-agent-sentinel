import sys
import json
from typing import Optional
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel, Field
from agent.graph import build_sentinel_graph

app = FastAPI(
    title="Onchain Agent Sentinel",
    description="Autonomous LangGraph Agent API for EVM Forensics and HTTP-402 Micro-settlement",
    version="0.1.0",
)

class QueryRequest(BaseModel):
    query: str = Field(..., example="Audit contract security for 0xd8dA6BF26964aF9D7eEd9e03E53415D37aA96045 on mainnet")
    caller_id: Optional[str] = Field("external-client", example="agent-node-01")
    payment_proof: Optional[str] = Field(None, example="mock_tx_settled")

@app.get("/health")
def health_check():
    return {"status": "ok", "service": "onchain-agent-sentinel", "version": "0.1.0"}

@app.post("/agent/invoke")
def invoke_agent(req: QueryRequest):
    graph = build_sentinel_graph()
    initial_state = {
        "query": req.query,
        "caller_id": req.caller_id,
        "payment_proof": req.payment_proof,
        "logs": [],
    }

    result = graph.invoke(initial_state)

    if result.get("payment_required") and not result.get("payment_verified"):
        return {
            "status": "PAYMENT_REQUIRED",
            "challenge": result.get("payment_challenge"),
            "trace": result.get("logs"),
        }

    return {
        "status": "SUCCESS",
        "output": result.get("final_output"),
        "trace": result.get("logs"),
    }

def cli_main():
    """CLI runner for direct local invocation."""
    query = sys.argv[1] if len(sys.argv) > 1 else "Inspect 0xd8dA6BF26964aF9D7eEd9e03E53415D37aA96045 on mainnet"
    print(f"\n[🚀] Initializing Onchain Sentinel Agent...")
    print(f"[📝] Query: {query}\n")

    graph = build_sentinel_graph()
    res = graph.invoke({
        "query": query,
        "caller_id": "cli-user",
        "payment_proof": "mock_tx_settled",
        "logs": [],
    })

    print(json.dumps(res.get("final_output", res), indent=2))

if __name__ == "__main__":
    cli_main()
