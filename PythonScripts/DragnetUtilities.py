from sqlalchemy import MetaData, Table, Column, Float, Integer, DateTime, SmallInteger, JSON, inspect, text
from sqlalchemy.dialects.mysql import insert
import pandas as pd

def create_table(engine, table_name):
    metadata = MetaData()
    ticker_table = Table(
        table_name,
        metadata,
        Column('timestamp', DateTime, primary_key=True),
        Column('market_price', Float),
        Column('ask_price', Float),
        Column('bid_price', Float),
        Column('open_price', Float),
        Column('high_price', Float),
        Column('low_price', Float),
        Column('close_price', Float),
        Column('volume', Integer),
        Column('RSI', Float),
        Column('MACD', Float),
        Column('MACD_Signal', Float),
        Column('MACD_Hist', Float),
        Column('SMA', Float),
        Column('EMA12P', Float),
        Column('EMA26P', Float),
        Column('EMA50P', Float),
        Column('EMA200P', Float),
        Column('Bollinger_Middle', Float),
        Column('Bollinger_Upper', Float),
        Column('Bollinger_Lower', Float),
        Column('Stochastic_Oscillator_K', Float),
        Column('Stochastic_Oscillator_D', Float),
        Column('Chicku_Span', Float),
        Column('Tenken_Sen', Float),
        Column('Kijun_Sen', Float),
        Column('Senkou_A', Float),
        Column('Senkou_B', Float),
        Column('Fib100%', Float),
        Column('Fib78.6%', Float),
        Column('Fib61.8%', Float),
        Column('Fib50%', Float),
        Column('Fib38.2%', Float),
        Column('Fib23.6%', Float),
        Column('Fib0%', Float),
        Column('ATR7P', Float),
        Column('ATR14P', Float),
        Column('ATR20P', Float),
        Column('ATR50P', Float),
        Column('ATR100P', Float),

        Column('open_priceM5', Float),
        Column('high_priceM5', Float),
        Column('low_priceM5', Float),
        Column('close_priceM5', Float),
        Column('volumeM5', Integer),
        Column('RSIM5', Float),
        Column('MACDM5', Float),
        Column('MACD_SignalM5', Float),
        Column('MACD_HistM5', Float),
        Column('SMAM5', Float),
        Column('EMA12PM5', Float),
        Column('EMA26PM5', Float),
        Column('EMA50PM5', Float),
        Column('EMA200PM5', Float),
        Column('Bollinger_MiddleM5', Float),
        Column('Bollinger_UpperM5', Float),
        Column('Bollinger_LowerM5', Float),
        Column('Stochastic_Oscillator_KM5', Float),
        Column('Stochastic_Oscillator_DM5', Float),
        Column('Chicku_SpanM5', Float),
        Column('Tenken_SenM5', Float),
        Column('Kijun_SenM5', Float),
        Column('Senkou_AM5', Float),
        Column('Senkou_BM5', Float),
        Column('Fib100%M5', Float),
        Column('Fib78.6%M5', Float),
        Column('Fib61.8%M5', Float),
        Column('Fib50%M5', Float),
        Column('Fib38.2%M5', Float),
        Column('Fib23.6%M5', Float),
        Column('Fib0%M5', Float),
        Column('ATR7PM5', Float),
        Column('ATR14PM5', Float),
        Column('ATR20PM5', Float),
        Column('ATR50PM5', Float),
        Column('ATR100PM5', Float),

        Column('open_priceM15', Float),
        Column('high_priceM15', Float),
        Column('low_priceM15', Float),
        Column('close_priceM15', Float),
        Column('volumeM15', Integer),
        Column('RSIM15', Float),
        Column('MACDM15', Float),
        Column('MACD_SignalM15', Float),
        Column('MACD_HistM15', Float),
        Column('SMAM15', Float),
        Column('EMA12PM15', Float),
        Column('EMA26PM15', Float),
        Column('EMA50PM15', Float),
        Column('EMA200PM15', Float),
        Column('Bollinger_MiddleM15', Float),
        Column('Bollinger_UpperM15', Float),
        Column('Bollinger_LowerM15', Float),
        Column('Stochastic_Oscillator_KM15', Float),
        Column('Stochastic_Oscillator_DM15', Float),
        Column('Chicku_SpanM15', Float),
        Column('Tenken_SenM15', Float),
        Column('Kijun_SenM15', Float),
        Column('Senkou_AM15', Float),
        Column('Senkou_BM15', Float),
        Column('Fib100%M15', Float),
        Column('Fib78.6%M15', Float),
        Column('Fib61.8%M15', Float),
        Column('Fib50%M15', Float),
        Column('Fib38.2%M15', Float),
        Column('Fib23.6%M15', Float),
        Column('Fib0%M15', Float),
        Column('ATR7PM15', Float),
        Column('ATR14PM15', Float),
        Column('ATR20PM15', Float),
        Column('ATR50PM15', Float),
        Column('ATR100PM15', Float),

        Column('open_priceM60', Float),
        Column('high_priceM60', Float),
        Column('low_priceM60', Float),
        Column('close_priceM60', Float),
        Column('volumeM60', Integer),
        Column('RSIM60', Float),
        Column('MACDM60', Float),
        Column('MACD_SignalM60', Float),
        Column('MACD_HistM60', Float),
        Column('SMAM60', Float),
        Column('EMA12PM60', Float),
        Column('EMA26PM60', Float),
        Column('EMA50PM60', Float),
        Column('EMA200PM60', Float),
        Column('Bollinger_MiddleM60', Float),
        Column('Bollinger_UpperM60', Float),
        Column('Bollinger_LowerM60', Float),
        Column('Stochastic_Oscillator_KM60', Float),
        Column('Stochastic_Oscillator_DM60', Float),
        Column('Chicku_SpanM60', Float),
        Column('Tenken_SenM60', Float),
        Column('Kijun_SenM60', Float),
        Column('Senkou_AM60', Float),
        Column('Senkou_BM60', Float),
        Column('Fib100%M60', Float),
        Column('Fib78.6%M60', Float),
        Column('Fib61.8%M60', Float),
        Column('Fib50%M60', Float),
        Column('Fib38.2%M60', Float),
        Column('Fib23.6%M60', Float),
        Column('Fib0%M60', Float),
        Column('ATR7PM60', Float),
        Column('ATR14PM60', Float),
        Column('ATR20PM60', Float),
        Column('ATR50PM60', Float),
        Column('ATR100PM60', Float),

        Column('open_priceM1440', Float),
        Column('high_priceM1440', Float),
        Column('low_priceM1440', Float),
        Column('close_priceM1440', Float),
        Column('volumeM1440', Integer),
        Column('RSIM1440', Float),
        Column('MACDM1440', Float),
        Column('MACD_SignalM1440', Float),
        Column('MACD_HistM1440', Float),
        Column('SMAM1440', Float),
        Column('EMA12PM1440', Float),
        Column('EMA26PM1440', Float),
        Column('EMA50PM1440', Float),
        Column('EMA200PM1440', Float),
        Column('Bollinger_MiddleM1440', Float),
        Column('Bollinger_UpperM1440', Float),
        Column('Bollinger_LowerM1440', Float),
        Column('Stochastic_Oscillator_KM1440', Float),
        Column('Stochastic_Oscillator_DM1440', Float),
        Column('Chicku_SpanM1440', Float),
        Column('Tenken_SenM1440', Float),
        Column('Kijun_SenM1440', Float),
        Column('Senkou_AM1440', Float),
        Column('Senkou_BM1440', Float),
        Column('Fib100%M1440', Float),
        Column('Fib78.6%M1440', Float),
        Column('Fib61.8%M1440', Float),
        Column('Fib50%M1440', Float),
        Column('Fib38.2%M1440', Float),
        Column('Fib23.6%M1440', Float),
        Column('Fib0%M1440', Float),
        Column('ATR7PM1440', Float),
        Column('ATR14PM1440', Float),
        Column('ATR20PM1440', Float),
        Column('ATR50PM1440', Float),
        Column('ATR100PM1440', Float),

        Column('order_book', JSON, nullable=True),

        # Order book-related indicators for the first exchange
        Column('bidask_spread', Float),
        Column('buy_sell_ratio', Float),
        Column('order_flow_imbalance', Float),
        Column('cumulative_volume_delta', Float),
        Column('price_impact', Float),
        Column('top_of_book_depth', JSON, nullable=True),  # Change to JSON
        Column('liquidity_imbalance', Float),
        Column('support', Float),
        Column('resistance', Float),
        Column('vwap', Float),
        Column('order_book_pressure', Float),
        Column('market_depth', JSON, nullable=True),
        Column('order_book_slope', JSON, nullable=True),

        Column('market_price2', Float),
        Column('ask_price2', Float),
        Column('bid_price2', Float),
        Column('open_price2', Float),
        Column('high_price2', Float),
        Column('low_price2', Float),
        Column('close_price2', Float),
        Column('volume2', Float),
        Column('RSI2', Float),
        Column('MACD2', Float),
        Column('MACD_Signal2', Float),
        Column('MACD_Hist2', Float),
        Column('SMA2', Float),
        Column('EMA12P2', Float),
        Column('EMA26P2', Float),
        Column('EMA50P2', Float),
        Column('EMA200P2', Float),
        Column('Bollinger_Middle2', Float),
        Column('Bollinger_Upper2', Float),
        Column('Bollinger_Lower2', Float),
        Column('Stochastic_Oscillator_K2', Float),
        Column('Stochastic_Oscillator_D2', Float),
        Column('Chicku_Span2', Float),
        Column('Tenken_Sen2', Float),
        Column('Kijun_Sen2', Float),
        Column('Senkou_A2', Float),
        Column('Senkou_B2', Float),
        Column('Fib100%2', Float),
        Column('Fib78.6%2', Float),
        Column('Fib61.8%2', Float),
        Column('Fib50%2', Float),
        Column('Fib38.2%2', Float),
        Column('Fib23.6%2', Float),
        Column('Fib0%2', Float),
        Column('ATR7P2', Float),
        Column('ATR14P2', Float),
        Column('ATR20P2', Float),
        Column('ATR50P2', Float),
        Column('ATR100P2', Float),

        Column('open_price2M5', Float),
        Column('high_price2M5', Float),
        Column('low_price2M5', Float),
        Column('close_price2M5', Float),
        Column('volume2M5', Integer),
        Column('RSI2M5', Float),
        Column('MACD2M5', Float),
        Column('MACD_Signal2M5', Float),
        Column('MACD_Hist2M5', Float),
        Column('SMA2M5', Float),
        Column('EMA12P2M5', Float),
        Column('EMA26P2M5', Float),
        Column('EMA50P2M5', Float),
        Column('EMA200P2M5', Float),
        Column('Bollinger_Middle2M5', Float),
        Column('Bollinger_Upper2M5', Float),
        Column('Bollinger_Lower2M5', Float),
        Column('Stochastic_Oscillator_K2M5', Float),
        Column('Stochastic_Oscillator_D2M5', Float),
        Column('Chicku_Span2M5', Float),
        Column('Tenken_Sen2M5', Float),
        Column('Kijun_Sen2M5', Float),
        Column('Senkou_A2M5', Float),
        Column('Senkou_B2M5', Float),
        Column('Fib100%2M5', Float),
        Column('Fib78.6%2M5', Float),
        Column('Fib61.8%2M5', Float),
        Column('Fib50%2M5', Float),
        Column('Fib38.2%2M5', Float),
        Column('Fib23.6%2M5', Float),
        Column('Fib0%2M5', Float),
        Column('ATR7P2M5', Float),
        Column('ATR14P2M5', Float),
        Column('ATR20P2M5', Float),
        Column('ATR50P2M5', Float),
        Column('ATR100P2M5', Float),

        Column('open_price2M15', Float),
        Column('high_price2M15', Float),
        Column('low_price2M15', Float),
        Column('close_price2M15', Float),
        Column('volume2M15', Integer),
        Column('RSI2M15', Float),
        Column('MACD2M15', Float),
        Column('MACD_Signal2M15', Float),
        Column('MACD_Hist2M15', Float),
        Column('SMA2M15', Float),
        Column('EMA12P2M15', Float),
        Column('EMA26P2M15', Float),
        Column('EMA50P2M15', Float),
        Column('EMA200P2M15', Float),
        Column('Bollinger_Middle2M15', Float),
        Column('Bollinger_Upper2M15', Float),
        Column('Bollinger_Lower2M15', Float),
        Column('Stochastic_Oscillator_K2M15', Float),
        Column('Stochastic_Oscillator_D2M15', Float),
        Column('Chicku_Span2M15', Float),
        Column('Tenken_Sen2M15', Float),
        Column('Kijun_Sen2M15', Float),
        Column('Senkou_A2M15', Float),
        Column('Senkou_B2M15', Float),
        Column('Fib100%2M15', Float),
        Column('Fib78.6%2M15', Float),
        Column('Fib61.8%2M15', Float),
        Column('Fib50%2M15', Float),
        Column('Fib38.2%2M15', Float),
        Column('Fib23.6%2M15', Float),
        Column('Fib0%2M15', Float),
        Column('ATR7P2M15', Float),
        Column('ATR14P2M15', Float),
        Column('ATR20P2M15', Float),
        Column('ATR50P2M15', Float),
        Column('ATR100P2M15', Float),

        Column('open_price2M60', Float),
        Column('high_price2M60', Float),
        Column('low_price2M60', Float),
        Column('close_price2M60', Float),
        Column('volume2M60', Integer),
        Column('RSI2M60', Float),
        Column('MACD2M60', Float),
        Column('MACD_Signal2M60', Float),
        Column('MACD_Hist2M60', Float),
        Column('SMA2M60', Float),
        Column('EMA12P2M60', Float),
        Column('EMA26P2M60', Float),
        Column('EMA50P2M60', Float),
        Column('EMA200P2M60', Float),
        Column('Bollinger_Middle2M60', Float),
        Column('Bollinger_Upper2M60', Float),
        Column('Bollinger_Lower2M60', Float),
        Column('Stochastic_Oscillator_K2M60', Float),
        Column('Stochastic_Oscillator_D2M60', Float),
        Column('Chicku_Span2M60', Float),
        Column('Tenken_Sen2M60', Float),
        Column('Kijun_Sen2M60', Float),
        Column('Senkou_A2M60', Float),
        Column('Senkou_B2M60', Float),
        Column('Fib100%2M60', Float),
        Column('Fib78.6%2M60', Float),
        Column('Fib61.8%2M60', Float),
        Column('Fib50%2M60', Float),
        Column('Fib38.2%2M60', Float),
        Column('Fib23.6%2M60', Float),
        Column('Fib0%2M60', Float),
        Column('ATR7P2M60', Float),
        Column('ATR14P2M60', Float),
        Column('ATR20P2M60', Float),
        Column('ATR50P2M60', Float),
        Column('ATR100P2M60', Float),

        Column('open_price2M1440', Float),
        Column('high_price2M1440', Float),
        Column('low_price2M1440', Float),
        Column('close_price2M1440', Float),
        Column('volume2M1440', Integer),
        Column('RSI2M1440', Float),
        Column('MACD2M1440', Float),
        Column('MACD_Signal2M1440', Float),
        Column('MACD_Hist2M1440', Float),
        Column('SMA2M1440', Float),
        Column('EMA12P2M1440', Float),
        Column('EMA26P2M1440', Float),
        Column('EMA50P2M1440', Float),
        Column('EMA200P2M1440', Float),
        Column('Bollinger_Middle2M1440', Float),
        Column('Bollinger_Upper2M1440', Float),
        Column('Bollinger_Lower2M1440', Float),
        Column('Stochastic_Oscillator_K2M1440', Float),
        Column('Stochastic_Oscillator_D2M1440', Float),
        Column('Chicku_Span2M1440', Float),
        Column('Tenken_Sen2M1440', Float),
        Column('Kijun_Sen2M1440', Float),
        Column('Senkou_A2M1440', Float),
        Column('Senkou_B2M1440', Float),
        Column('Fib100%2M1440', Float),
        Column('Fib78.6%2M1440', Float),
        Column('Fib61.8%2M1440', Float),
        Column('Fib50%2M1440', Float),
        Column('Fib38.2%2M1440', Float),
        Column('Fib23.6%2M1440', Float),
        Column('Fib0%2M1440', Float),
        Column('ATR7P2M1440', Float),
        Column('ATR14P2M1440', Float),
        Column('ATR20P2M1440', Float),
        Column('ATR50P2M1440', Float),
        Column('ATR100P2M1440', Float),

        Column('order_book2', JSON, nullable=True),

        # Order book-related indicators for the second exchange
        Column('bidask_spread2', Float),
        Column('buy_sell_ratio2', Float),
        Column('order_flow_imbalance2', Float),
        Column('cumulative_volume_delta2', Float),
        Column('price_impact2', Float),
        Column('top_of_book_depth2', JSON, nullable=True),  # Change to JSON
        Column('liquidity_imbalance2', Float),
        Column('support2', Float),
        Column('resistance2', Float),
        Column('vwap2', Float),
        Column('order_book_pressure2', Float),
        Column('market_depth2', JSON, nullable=True),
        Column('order_book_slope2', JSON, nullable=True),

        Column('market_price3', Float),
        Column('ask_price3', Float),
        Column('bid_price3', Float),
        Column('open_price3', Float),
        Column('high_price3', Float),
        Column('low_price3', Float),
        Column('close_price3', Float),
        Column('volume3', Float),
        Column('RSI3', Float),
        Column('MACD3', Float),
        Column('MACD_Signal3', Float),
        Column('MACD_Hist3', Float),
        Column('SMA3', Float),
        Column('EMA12P3', Float),
        Column('EMA26P3', Float),
        Column('EMA50P3', Float),
        Column('EMA200P3', Float),
        Column('Bollinger_Middle3', Float),
        Column('Bollinger_Upper3', Float),
        Column('Bollinger_Lower3', Float),
        Column('Stochastic_Oscillator_K3', Float),
        Column('Stochastic_Oscillator_D3', Float),
        Column('Chicku_Span3', Float),
        Column('Tenken_Sen3', Float),
        Column('Kijun_Sen3', Float),
        Column('Senkou_A3', Float),
        Column('Senkou_B3', Float),
        Column('Fib100%3', Float),
        Column('Fib78.6%3', Float),
        Column('Fib61.8%3', Float),
        Column('Fib50%3', Float),
        Column('Fib38.2%3', Float),
        Column('Fib23.6%3', Float),
        Column('Fib0%3', Float),
        Column('ATR7P3', Float),
        Column('ATR14P3', Float),
        Column('ATR20P3', Float),
        Column('ATR50P3', Float),
        Column('ATR100P3', Float),

        Column('open_price3M5', Float),
        Column('high_price3M5', Float),
        Column('low_price3M5', Float),
        Column('close_price3M5', Float),
        Column('volume3M5', Integer),
        Column('RSI3M5', Float),
        Column('MACD3M5', Float),
        Column('MACD_Signal3M5', Float),
        Column('MACD_Hist3M5', Float),
        Column('SMA3M5', Float),
        Column('EMA12P3M5', Float),
        Column('EMA26P3M5', Float),
        Column('EMA50P3M5', Float),
        Column('EMA200P3M5', Float),
        Column('Bollinger_Middle3M5', Float),
        Column('Bollinger_Upper3M5', Float),
        Column('Bollinger_Lower3M5', Float),
        Column('Stochastic_Oscillator_K3M5', Float),
        Column('Stochastic_Oscillator_D3M5', Float),
        Column('Chicku_Span3M5', Float),
        Column('Tenken_Sen3M5', Float),
        Column('Kijun_Sen3M5', Float),
        Column('Senkou_A3M5', Float),
        Column('Senkou_B3M5', Float),
        Column('Fib100%3M5', Float),
        Column('Fib78.6%3M5', Float),
        Column('Fib61.8%3M5', Float),
        Column('Fib50%3M5', Float),
        Column('Fib38.2%3M5', Float),
        Column('Fib23.6%3M5', Float),
        Column('Fib0%3M5', Float),
        Column('ATR7P3M5', Float),
        Column('ATR14P3M5', Float),
        Column('ATR20P3M5', Float),
        Column('ATR50P3M5', Float),
        Column('ATR100P3M5', Float),

        Column('open_price3M15', Float),
        Column('high_price3M15', Float),
        Column('low_price3M15', Float),
        Column('close_price3M15', Float),
        Column('volume3M15', Integer),
        Column('RSI3M15', Float),
        Column('MACD3M15', Float),
        Column('MACD_Signal3M15', Float),
        Column('MACD_Hist3M15', Float),
        Column('SMA3M15', Float),
        Column('EMA12P3M15', Float),
        Column('EMA26P3M15', Float),
        Column('EMA50P3M15', Float),
        Column('EMA200P3M15', Float),
        Column('Bollinger_Middle3M15', Float),
        Column('Bollinger_Upper3M15', Float),
        Column('Bollinger_Lower3M15', Float),
        Column('Stochastic_Oscillator_K3M15', Float),
        Column('Stochastic_Oscillator_D3M15', Float),
        Column('Chicku_Span3M15', Float),
        Column('Tenken_Sen3M15', Float),
        Column('Kijun_Sen3M15', Float),
        Column('Senkou_A3M15', Float),
        Column('Senkou_B3M15', Float),
        Column('Fib100%3M15', Float),
        Column('Fib78.6%3M15', Float),
        Column('Fib61.8%3M15', Float),
        Column('Fib50%3M15', Float),
        Column('Fib38.2%3M15', Float),
        Column('Fib23.6%3M15', Float),
        Column('Fib0%3M15', Float),
        Column('ATR7P3M15', Float),
        Column('ATR14P3M15', Float),
        Column('ATR20P3M15', Float),
        Column('ATR50P3M15', Float),
        Column('ATR100P3M15', Float),

        Column('open_price3M60', Float),
        Column('high_price3M60', Float),
        Column('low_price3M60', Float),
        Column('close_price3M60', Float),
        Column('volume3M60', Integer),
        Column('RSI3M60', Float),
        Column('MACD3M60', Float),
        Column('MACD_Signal3M60', Float),
        Column('MACD_Hist3M60', Float),
        Column('SMA3M60', Float),
        Column('EMA12P3M60', Float),
        Column('EMA26P3M60', Float),
        Column('EMA50P3M60', Float),
        Column('EMA200P3M60', Float),
        Column('Bollinger_Middle3M60', Float),
        Column('Bollinger_Upper3M60', Float),
        Column('Bollinger_Lower3M60', Float),
        Column('Stochastic_Oscillator_K3M60', Float),
        Column('Stochastic_Oscillator_D3M60', Float),
        Column('Chicku_Span3M60', Float),
        Column('Tenken_Sen3M60', Float),
        Column('Kijun_Sen3M60', Float),
        Column('Senkou_A3M60', Float),
        Column('Senkou_B3M60', Float),
        Column('Fib100%3M60', Float),
        Column('Fib78.6%3M60', Float),
        Column('Fib61.8%3M60', Float),
        Column('Fib50%3M60', Float),
        Column('Fib38.2%3M60', Float),
        Column('Fib23.6%3M60', Float),
        Column('Fib0%3M60', Float),
        Column('ATR7P3M60', Float),
        Column('ATR14P3M60', Float),
        Column('ATR20P3M60', Float),
        Column('ATR50P3M60', Float),
        Column('ATR100P3M60', Float),

        Column('open_price3M1440', Float),
        Column('high_price3M1440', Float),
        Column('low_price3M1440', Float),
        Column('close_price3M1440', Float),
        Column('volume3M1440', Integer),
        Column('RSI3M1440', Float),
        Column('MACD3M1440', Float),
        Column('MACD_Signal3M1440', Float),
        Column('MACD_Hist3M1440', Float),
        Column('SMA3M1440', Float),
        Column('EMA12P3M1440', Float),
        Column('EMA26P3M1440', Float),
        Column('EMA50P3M1440', Float),
        Column('EMA200P3M1440', Float),
        Column('Bollinger_Middle3M1440', Float),
        Column('Bollinger_Upper3M1440', Float),
        Column('Bollinger_Lower3M1440', Float),
        Column('Stochastic_Oscillator_K3M1440', Float),
        Column('Stochastic_Oscillator_D3M1440', Float),
        Column('Chicku_Span3M1440', Float),
        Column('Tenken_Sen3M1440', Float),
        Column('Kijun_Sen3M1440', Float),
        Column('Senkou_A3M1440', Float),
        Column('Senkou_B3M1440', Float),
        Column('Fib100%3M1440', Float),
        Column('Fib78.6%3M1440', Float),
        Column('Fib61.8%3M1440', Float),
        Column('Fib50%3M1440', Float),
        Column('Fib38.2%3M1440', Float),
        Column('Fib23.6%3M1440', Float),
        Column('Fib0%3M1440', Float),
        Column('ATR7P3M1440', Float),
        Column('ATR14P3M1440', Float),
        Column('ATR20P3M1440', Float),
        Column('ATR50P3M1440', Float),
        Column('ATR100P3M1440', Float),

        Column('order_book3', JSON, nullable=True),

        # Order book-related indicators for the first exchange
        Column('bidask_spread3', Float),
        Column('buy_sell_ratio3', Float),
        Column('order_flow_imbalance3', Float),
        Column('cumulative_volume_delta3', Float),
        Column('price_impact3', Float),
        Column('top_of_book_depth3', JSON, nullable=True),  # Change to JSON
        Column('liquidity_imbalance3', Float),
        Column('support3', Float),
        Column('resistance3', Float),
        Column('vwap3', Float),
        Column('order_book_pressure3', Float),
        Column('market_depth3', JSON, nullable=True),
        Column('order_book_slope3', JSON, nullable=True),

        Column('google_trends_score', SmallInteger),
        Column('news_p_score', Float),
        Column('news_n_score', Float),
        Column('news_r_score', Float),
        Column('news_tf_score', Float),
        Column('news_tp_score', Float),
        Column('news_sr_score', Float),
        Column('sm_p_score', Float),
        Column('sm_n_score', Float),
        Column('sm_r_score', Float),
        Column('sm_tf_score', Float),
        Column('sm_tp_score', Float),
        Column('sm_sr_score', Float),
        Column('price_action', JSON, nullable=True),
        Column('VIPTrades', JSON, nullable=True),
        Column('Curated', Integer, default=0)
    )
    metadata.create_all(engine)
    return ticker_table

def check_if_table_exists(engine, table_name):
    inspector = inspect(engine)
    return table_name in inspector.get_table_names()

def insert_crypto_data(engine, table, batch_rows):
    with engine.begin() as conn:
        for row in batch_rows:
            # Check if row exists
            existing = conn.execute(
                table.select().where(table.c.timestamp == row['timestamp'])
            ).fetchone()

            if not existing:
                # Partial Insert: only columns that are present & not None
                insert_dict = {k: v for k, v in row.items() if v is not None}
                stmt = insert(table).values(**insert_dict)
                conn.execute(stmt)
            else:
                # Partial Update: only columns that are present & not None
                update_dict = {k: v for k, v in row.items() if k != 'timestamp' and v is not None}
                if update_dict:
                    stmt = (
                        table.update()
                        .where(table.c.timestamp == row['timestamp'])
                        .values(**update_dict)
                    )
                    conn.execute(stmt)

def insert_crypto_data_retro(engine, table, rows, allowed_columns=None, chunk=2000):
    if not rows:
        return

    # Decide columns once
    cols = sorted({k for r in rows for k in r.keys()})
    if allowed_columns is not None:
        cols = [c for c in cols if c in allowed_columns or c == 'timestamp']

    col_list   = ", ".join(f"`{c}`" for c in cols)
    values_tpl = ", ".join(f":{c}" for c in cols)
    updates    = ", ".join(f"`{c}`=VALUES(`{c}`)" for c in cols if c != 'timestamp')

    sql = text(
        f"INSERT INTO `{table.name}` ({col_list}) "
        f"VALUES ({values_tpl}) "
        f"ON DUPLICATE KEY UPDATE {updates}"
    )

    # Executemany in chunks
    with engine.begin() as conn:
        for i in range(0, len(rows), chunk):
            batch = rows[i:i+chunk]
            # ensure all keys exist; keep None for missing
            payload = [{c: r.get(c, None) for c in cols} for r in batch]
            conn.execute(sql, payload)

def aggregate_interval_candles(merged_df, interval_val, suffix=""):
    df = merged_df.copy()

    # --- Required base columns for this suffix ---
    open_col  = 'open_price'  if suffix == "" else f'open_price{suffix}'
    high_col  = 'high_price'  if suffix == "" else f'high_price{suffix}'
    low_col   = 'low_price'   if suffix == "" else f'low_price{suffix}'
    close_col = 'close_price' if suffix == "" else f'close_price{suffix}'
    vol_col   = 'volume'      if suffix == "" else f'volume{suffix}'

    need = [open_col, high_col, low_col, close_col, vol_col]
    missing = [c for c in need if c not in df.columns]
    if missing:
        print(f"[aggregate] missing {missing} for suffix '{suffix}' @ {interval_val}m")
        return pd.DataFrame()

    # Ensure timestamp is datetime (naive) and build cutoff
    if not pd.api.types.is_datetime64_any_dtype(df['timestamp']):
        df['timestamp'] = pd.to_datetime(df['timestamp'], unit='s', errors='coerce')
    df['timestamp'] = df['timestamp'].dt.tz_localize(None)
    df['interval_cutoff'] = df['timestamp'].dt.floor(f'{interval_val}min')  # 'T' -> 'min'

    agg_open  = f'open_price{suffix}M{interval_val}'
    agg_high  = f'high_price{suffix}M{interval_val}'
    agg_low   = f'low_price{suffix}M{interval_val}'
    agg_close = f'close_price{suffix}M{interval_val}'
    agg_vol   = f'volume{suffix}M{interval_val}'

    aggregated_rows = []
    last_close = None

    groups = list(df.groupby('interval_cutoff'))
    groups.sort(key=lambda x: x[0])

    for cutoff, group in groups:
        group = group.sort_values('timestamp').reset_index(drop=True)

        # Candle open: first non-null open of the group, else fallback to last_close
        candle_open = None
        if open_col in group.columns:
            first_nonnull_open = group[open_col].dropna()
            if not first_nonnull_open.empty:
                candle_open = first_nonnull_open.iloc[0]
        if candle_open is None:
            candle_open = last_close  # may be None for first group

        # High/Low/Close/Vol with guards
        candle_high  = group[high_col].max() if high_col in group.columns else None
        candle_low   = group[low_col].min()  if low_col  in group.columns else None
        candle_close = group[close_col].iloc[-1] if close_col in group.columns else None
        candle_vol   = group[vol_col].sum()  if vol_col  in group.columns else None

        aggregated_rows.append({
            'timestamp': group.iloc[-1]['timestamp'],
            agg_open:  candle_open,
            agg_high:  candle_high,
            agg_low:   candle_low,
            agg_close: candle_close,
            agg_vol:   candle_vol
        })

        if candle_close is not None:
            last_close = candle_close

    return pd.DataFrame(aggregated_rows)

def calculate_indicators_for_interval(df, interval):
    suffixes = ['', '2', '3']
    new_cols = {}  # COLLECT ALL NEW COLUMNS HERE

    for suffix in suffixes:
        close_col = f'close_price{suffix}{interval}'
        high_col  = f'high_price{suffix}{interval}'
        low_col   = f'low_price{suffix}{interval}'

        if not all(c in df.columns for c in (close_col, high_col, low_col)):
            continue
        if not df[close_col].notnull().any():
            continue

        rsi_col = f'RSI{suffix}{interval}'
        new_cols[rsi_col] = calculate_rsi(df[close_col])

        macd_col      = f'MACD{suffix}{interval}'
        macd_sig_col  = f'MACD_Signal{suffix}{interval}'
        macd_hist_col = f'MACD_Hist{suffix}{interval}'
        macd_line, sig_line, hist = calculate_macd(df[close_col])
        new_cols[macd_col] = macd_line
        new_cols[macd_sig_col] = sig_line
        new_cols[macd_hist_col] = hist

        sma_col = f'SMA{suffix}{interval}'
        new_cols[sma_col] = calculate_sma(df[close_col])

        bb = calculate_bollinger_bands(df[close_col], period=20, num_std_dev=2)
        new_cols[f'Bollinger_Middle{suffix}{interval}'] = bb['Middle']
        new_cols[f'Bollinger_Upper{suffix}{interval}']  = bb['Upper']
        new_cols[f'Bollinger_Lower{suffix}{interval}']  = bb['Lower']

        k_col = f'Stochastic_Oscillator_K{suffix}{interval}'
        d_col = f'Stochastic_Oscillator_D{suffix}{interval}'
        percent_k, percent_d = calculate_stochastic_oscillator(df, high_col, low_col, close_col)
        new_cols[k_col] = percent_k
        new_cols[d_col] = percent_d

        ema_dict = calculate_ema(df[close_col])
        for ema_key, ema_series in ema_dict.items():
            new_cols[f'{ema_key}{suffix}{interval}'] = ema_series

        ichimoku_df = calculate_ichimoku(df, high_col, low_col, close_col, suffix=f'{suffix}{interval}')
        for col in ichimoku_df.columns:
            if col not in df.columns:
                new_cols[col] = ichimoku_df[col]

        atr_values = calculate_atr_values(df[high_col], df[low_col], df[close_col])
        for atr_key, atr_series in atr_values.items():
            new_cols[f'{atr_key}{suffix}{interval}'] = atr_series

    # ADD ALL NEW COLUMNS AT ONCE
    if new_cols:
        new_cols_df = pd.DataFrame(new_cols, index=df.index)
        df = pd.concat([df, new_cols_df], axis=1)
        df = df.copy()  # De-fragment the dataframe (nice-to-have)

    return df


def calculate_ichimoku(df, high_col, low_col, close_col, suffix=''):
    period9_high = df[high_col].rolling(window=9).max()
    period9_low = df[low_col].rolling(window=9).min()
    df[f'Tenken_Sen{suffix}'] = (period9_high + period9_low) / 2

    period26_high = df[high_col].rolling(window=26).max()
    period26_low = df[low_col].rolling(window=26).min()
    df[f'Kijun_Sen{suffix}'] = (period26_high + period26_low) / 2

    df[f'Senkou_A{suffix}'] = ((df[f'Tenken_Sen{suffix}'] + df[f'Kijun_Sen{suffix}']) / 2).shift(26)

    period52_high = df[high_col].rolling(window=52).max()
    period52_low = df[low_col].rolling(window=52).min()
    df[f'Senkou_B{suffix}'] = ((period52_high + period52_low) / 2).shift(26)

    df[f'Chicku_Span{suffix}'] = df[close_col]
    return df

def calculate_rsi(data, period=14):
    delta = data.diff()
    gain = delta.where(delta > 0, 0)
    loss = -delta.where(delta < 0, 0)
    avg_gain = gain.rolling(window=period).mean()
    avg_loss = loss.rolling(window=period).mean()
    rs = avg_gain / avg_loss
    return 100 - (100 / (1 + rs))

def calculate_macd(data, short_period=12, long_period=26, signal_period=9):
    exp1 = data.ewm(span=short_period, adjust=False).mean()
    exp2 = data.ewm(span=long_period, adjust=False).mean()
    macd_line = exp1 - exp2
    signal_line = macd_line.ewm(span=signal_period, adjust=False).mean()
    macd_histogram = macd_line - signal_line
    return macd_line, signal_line, macd_histogram

def calculate_stochastic_oscillator(df, high_col, low_col, close_col, k_period=14, d_period=3):
    low_min = df[low_col].rolling(window=k_period).min()
    high_max = df[high_col].rolling(window=k_period).max()
    percent_k = 100 * (df[close_col] - low_min) / (high_max - low_min)
    percent_d = percent_k.rolling(window=d_period).mean()
    return percent_k, percent_d

def calculate_sma(data, period=15):
    return data.rolling(window=period).mean()

def calculate_ema(data, periods=[12, 26, 50, 200]):
    return {f'EMA{period}P': data.ewm(span=period, adjust=False).mean() for period in periods}

def calculate_atr_values(high_prices, low_prices, close_prices, periods=[7, 14, 20, 50, 100]):
    true_ranges = []
    for i in range(len(high_prices)):
        if i == 0:
            tr = high_prices.iloc[i] - low_prices.iloc[i]
        else:
            current_high = high_prices.iloc[i]
            current_low = low_prices.iloc[i]
            previous_close = close_prices.iloc[i - 1]
            tr = max(
                current_high - current_low,
                abs(current_high - previous_close),
                abs(current_low - previous_close)
            )
        true_ranges.append(tr)
    true_ranges = pd.Series(true_ranges, index=high_prices.index)
    atr_values = {}
    for period in periods:
        atr = true_ranges.rolling(window=period).mean()
        atr_values[f'ATR{period}P'] = atr
    return atr_values

def calculate_bollinger_bands(data, period=20, num_std_dev=2):
    sma = calculate_sma(data, period)
    rolling_std = data.rolling(window=period).std()
    upper_band = sma + (rolling_std * num_std_dev)
    lower_band = sma - (rolling_std * num_std_dev)
    return pd.DataFrame({'Middle': sma, 'Upper': upper_band, 'Lower': lower_band})

def aggregate_interval_candles_asof(merged_df, interval_min, suffix=""):
    df = merged_df.copy()

    open_col  = 'open_price'  if suffix == "" else f'open_price{suffix}'
    high_col  = 'high_price'  if suffix == "" else f'high_price{suffix}'
    low_col   = 'low_price'   if suffix == "" else f'low_price{suffix}'
    close_col = 'close_price' if suffix == "" else f'close_price{suffix}'
    vol_col   = 'volume'      if suffix == "" else f'volume{suffix}'

    need = [open_col, high_col, low_col, close_col, vol_col, 'timestamp']
    if any(c not in df.columns for c in need):
        return pd.DataFrame()

    # Ensure timestamp is datetime-naive
    if not pd.api.types.is_datetime64_any_dtype(df['timestamp']):
        df['timestamp'] = pd.to_datetime(df['timestamp'], unit='s', errors='coerce')
    df['timestamp'] = df['timestamp'].dt.tz_localize(None)
    df = df.sort_values('timestamp').reset_index(drop=True)

    # ---- CRITICAL: coerce numeric for price family before cummax/cummin ----
    df[open_col]  = pd.to_numeric(df[open_col],  errors='coerce')
    df[high_col]  = pd.to_numeric(df[high_col],  errors='coerce')
    df[low_col]   = pd.to_numeric(df[low_col],   errors='coerce')
    df[close_col] = pd.to_numeric(df[close_col], errors='coerce')
    df[vol_col]   = pd.to_numeric(df[vol_col],   errors='coerce').fillna(0.0)

    floor = df['timestamp'].dt.floor(f'{interval_min}min')
    df['_floor'] = floor
    g = df.groupby('_floor', sort=False)

    asof_open  = g[open_col].transform('first')
    asof_high  = g[high_col].cummax()
    asof_low   = g[low_col].cummin()
    asof_close = df[close_col]
    asof_vol   = g[vol_col].cumsum()

    out = pd.DataFrame({
        'timestamp': df['timestamp'],
        f'open_price{suffix}M{interval_min}':  asof_open,
        f'high_price{suffix}M{interval_min}':  asof_high,
        f'low_price{suffix}M{interval_min}':   asof_low,
        f'close_price{suffix}M{interval_min}': asof_close,
        f'volume{suffix}M{interval_min}':      asof_vol
    })
    return out
