import os
from typing import Dict, Any, Optional
from web3 import Web3

DEFAULT_RPC_URLS = {
    "mainnet": "https://cloudflare-eth.com",
    "sepolia": "https://rpc.sepolia.org",
    "base": "https://mainnet.base.org",
}

class BlockchainClient:
    """EVM RPC interface for querying on-chain state safely and deterministically."""

    def __init__(self, rpc_url: Optional[str] = None, network: str = "mainnet"):
        self.network = network
        self.rpc_url = rpc_url or os.getenv("ETH_RPC_URL") or DEFAULT_RPC_URLS.get(network, DEFAULT_RPC_URLS["mainnet"])
        self.w3 = Web3(Web3.HTTPProvider(self.rpc_url))

    def is_connected(self) -> bool:
        try:
            return self.w3.is_connected()
        except Exception:
            return False

    def validate_address(self, address: str) -> Optional[str]:
        """Validates and formats an address to checksum format."""
        if not address or not Web3.is_address(address):
            return None
        return Web3.to_checksum_address(address)

    def get_address_overview(self, address: str) -> Dict[str, Any]:
        """Fetches bytecode, balance, and transaction count for a given address."""
        checksum_addr = self.validate_address(address)
        if not checksum_addr:
            return {"error": f"Invalid address format: {address}"}

        try:
            balance_wei = self.w3.eth.get_balance(checksum_addr)
            balance_eth = float(self.w3.from_wei(balance_wei, "ether"))
            tx_count = self.w3.eth.get_transaction_count(checksum_addr)
            bytecode = self.w3.eth.get_code(checksum_addr).hex()
            is_contract = len(bytecode) > 2  # more than '0x'

            return {
                "address": checksum_addr,
                "network": self.network,
                "is_contract": is_contract,
                "balance_eth": round(balance_eth, 6),
                "balance_wei": str(balance_wei),
                "nonce_tx_count": tx_count,
                "bytecode_length_bytes": (len(bytecode) - 2) // 2 if is_contract else 0,
                "has_code": is_contract,
            }
        except Exception as e:
            return {
                "address": checksum_addr,
                "error": str(e),
                "is_contract": False,
                "balance_eth": 0.0,
                "nonce_tx_count": 0,
            }
