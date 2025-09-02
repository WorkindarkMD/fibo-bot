//+------------------------------------------------------------------+
//|                                        QuantumOscillatorEA.mq5 |
//|                                  Copyright 2023, Jules the AI |
//|                                             https://jules.ai |
//+------------------------------------------------------------------+
#property copyright "Copyright 2023, Jules the AI"
#property link      "https://jules.ai"
#property version   "1.02"
#property description "Торговый советник на основе стратегии 'Квантовый Осциллятор' (исправленная версия)"

#include <Trade\Trade.mqh>

//--- 🎯 Фильтр Рыночных Режимов
input group "Фильтр Рыночных Режимов"
input int      g_adx_len = 14;                  // Длина ADX
input int      g_atr_len = 14;                  // Длина ATR
input double   g_adx_threshold = 23.0;          // Порог трендовости ADX
input double   g_vol_threshold_pct = 0.05;      // Порог волатильности (%)

//--- ⚡ Квантовый Осциллятор
input group "Квантовый Осциллятор"
input int      g_rsi_len = 14;                  // Длина RSI
input int      g_mfi_len = 14;                  // Длина MFI
input int      g_mom_len = 10;                  // Длина Momentum
input int      g_norm_len = 50;                 // Период нормализации
input double   g_w_rsi = 0.4;                   // Вес RSI
input double   g_w_mfi = 0.4;                   // Вес MFI
input double   g_w_mom = 0.2;                   // Вес Momentum

//--- 📈 Логика Сигналов
input group "Логика Сигналов"
input double   g_trend_long_entry = 55.0;       // Уровень входа в лонг (тренд)
input double   g_trend_short_entry = 45.0;      // Уровень входа в шорт (тренд)
input double   g_range_long_entry = 20.0;       // Уровень входа в лонг (флет)
input double   g_range_short_entry = 80.0;      // Уровень входа в шорт (флет)

//--- 🛡️ Управление Рисками
input group "Управление Рисками"
input bool     g_use_sl = true;                 // Использовать Стоп-Лосс
input double   g_sl_pct = 2.0;                  // Стоп-Лосс (%)
input bool     g_use_tp = true;                 // Использовать Тейк-Профит
input double   g_tp_pct = 4.0;                  // Тейк-Профит (%)

//--- 🤖 Автоторговля
input group "Автоторговля"
input bool     g_enable_auto = true;            // Включить автоторговлю
input ulong    g_magic_number = 12345;          // Magic Number
input double   g_position_size_pct = 10.0;      // Размер позиции (% от депозита)


//--- Глобальные переменные
CTrade trade;

//--- Хендлы индикаторов
int g_adx_handle;
int g_atr_handle;
int g_rsi_handle;
int g_mfi_handle;
int g_mom_handle;

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- Инициализация торгового объекта
   trade.SetExpertMagicNumber(g_magic_number);
   trade.SetTypeFillingBySymbol(Symbol());

//--- Получение хендлов индикаторов
   g_adx_handle = iADX(_Symbol, _Period, g_adx_len);
   g_atr_handle = iATR(_Symbol, _Period, g_atr_len);
   g_rsi_handle = iRSI(_Symbol, _Period, g_rsi_len, PRICE_CLOSE);
   g_mfi_handle = iMFI(_Symbol, _Period, g_mfi_len, VOLUME_TICK);
   g_mom_handle = iMomentum(_Symbol, _Period, g_mom_len, PRICE_CLOSE);

   if(g_adx_handle == INVALID_HANDLE || g_atr_handle == INVALID_HANDLE || g_rsi_handle == INVALID_HANDLE || g_mfi_handle == INVALID_HANDLE || g_mom_handle == INVALID_HANDLE)
     {
      Print("Ошибка при создании хендлов индикаторов");
      return(INIT_FAILED);
     }

//---
   return(INIT_SUCCEEDED);
  }
//+------------------------------------------------------------------+
//| Expert deinitialization function                                 |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
  {
//--- Освобождение хендлов индикаторов
   IndicatorRelease(g_adx_handle);
   IndicatorRelease(g_atr_handle);
   IndicatorRelease(g_rsi_handle);
   IndicatorRelease(g_mfi_handle);
   IndicatorRelease(g_mom_handle);
  }
//+------------------------------------------------------------------+
//| Helper functions                                                 |
//+------------------------------------------------------------------+
//--- Crossover
bool Crossover(double current_val, double prev_val, double level)
  {
   return(prev_val < level && current_val > level);
  }
//--- Crossunder
bool Crossunder(double current_val, double prev_val, double level)
  {
   return(prev_val > level && current_val < level);
  }

//--- Calculate Quantum Oscillator for a specific bar
double GetQuantumOscillator(int bar_index)
  {
//--- Buffers
   double rsi_buf[], mfi_buf[], mom_buf[];
//--- Copy data
   if(CopyBuffer(g_rsi_handle, 0, bar_index, 1, rsi_buf) <= 0 ||
      CopyBuffer(g_mfi_handle, 0, bar_index, 1, mfi_buf) <= 0 ||
      CopyBuffer(g_mom_handle, 0, bar_index, g_norm_len, mom_buf) <= 0)
     {
      Print("Ошибка копирования буферов для GetQuantumOscillator");
      return -1; // Return error code
     }
//--- Calculations
   double rsi_val = rsi_buf[0];
   double mfi_val = mfi_buf[0];
   double mom_val = mom_buf[0];
//--- Normalize Momentum
   double mom_lowest = mom_buf[ArrayMinimum(mom_buf, 0, g_norm_len)];
   double mom_highest = mom_buf[ArrayMaximum(mom_buf, 0, g_norm_len)];
   double mom_range = mom_highest - mom_lowest;
   double norm_mom_val = (mom_range == 0) ? 50.0 : ((mom_val - mom_lowest) / mom_range) * 100.0;
//--- Final Oscillator
   double total_weight = g_w_rsi + g_w_mfi + g_w_mom;
   if(total_weight == 0) return 0;
   return (rsi_val * g_w_rsi + mfi_val * g_w_mfi + norm_mom_val * g_w_mom) / total_weight;
  }

//--- Calculate Lot Size
double CalculateLotSize()
  {
   double equity = AccountInfoDouble(ACCOUNT_EQUITY);
   double money_to_risk = equity * (g_position_size_pct / 100.0);

   MqlTick last_tick;
   SymbolInfoTick(_Symbol, last_tick);
   double price = last_tick.ask;
   if(price == 0) return 0.01;

   double contract_size = SymbolInfoDouble(_Symbol, SYMBOL_TRADE_CONTRACT_SIZE);
   double lot_size = money_to_risk / (contract_size * price);

//--- Normalize and check limits
   double min_lot = SymbolInfoDouble(_Symbol, SYMBOL_VOLUME_MIN);
   double max_lot = SymbolInfoDouble(_Symbol, SYMBOL_VOLUME_MAX);
   double lot_step = SymbolInfoDouble(_Symbol, SYMBOL_VOLUME_STEP);

   lot_size = MathFloor(lot_size / lot_step) * lot_step;
   if(lot_size < min_lot) lot_size = min_lot;
   if(lot_size > max_lot) lot_size = max_lot;

   return lot_size;
  }
//+------------------------------------------------------------------+
//| Expert tick function                                             |
//+------------------------------------------------------------------+
void OnTick()
  {
//--- Проверяем, разрешена ли торговля
   if(!MQLInfoInteger(MQL_TRADE_ALLOWED) || !g_enable_auto)
      return;

//--- Проверяем, новый ли бар, чтобы не выполнять логику на каждом тике
   static datetime prev_bar_time = 0;
   MqlRates current_rates[1];
   if(CopyRates(_Symbol, _Period, 0, 1, current_rates) <= 0) return;
   datetime current_bar_time = current_rates[0].time;

   if(current_bar_time == prev_bar_time)
      return;
   prev_bar_time = current_bar_time;

//--- Массивы для хранения данных индикаторов
   double adx_main_buf[], plus_di_buf[], minus_di_buf[], atr_buf[];
   if(CopyBuffer(g_adx_handle, MAIN_LINE, 1, 1, adx_main_buf) <= 0 ||
      CopyBuffer(g_adx_handle, PLUSDI_LINE, 1, 1, plus_di_buf) <= 0 ||
      CopyBuffer(g_adx_handle, MINUSDI_LINE, 1, 1, minus_di_buf) <= 0 ||
      CopyBuffer(g_atr_handle, 0, 1, 1, atr_buf) <= 0)
     {
      Print("Ошибка копирования данных из индикаторов режима");
      return;
     }

//--- Получаем цену закрытия
   MqlRates rate_buf[1];
   if(CopyRates(_Symbol, _Period, 1, 1, rate_buf) <= 0)
     {
      Print("Ошибка копирования ценовых данных");
      return;
     }

//--- МОДУЛЬ 1: ФИЛЬТР РЫНОЧНЫХ РЕЖИМОВ
   double normalized_atr = (atr_buf[0] / rate_buf[0].close) * 100;
   bool is_trending = adx_main_buf[0] > g_adx_threshold;
   bool is_volatile = normalized_atr > g_vol_threshold_pct;
   bool is_bullish_trend = is_trending && plus_di_buf[0] > minus_di_buf[0];
   bool is_bearish_trend = is_trending && plus_di_buf[0] < minus_di_buf[0];
   bool is_volatile_range = !is_trending && is_volatile;

//--- МОДУЛЬ 2: КВАНТОВЫЙ ОСЦИЛЛЯТОР
   double quantum_osc_current = GetQuantumOscillator(1); // bar 1 is the last completed bar
   double quantum_osc_prev = GetQuantumOscillator(2);    // bar 2 is the one before that
   if(quantum_osc_current < 0 || quantum_osc_prev < 0) return; // Error in calculation

//--- МОДУЛЬ 3: ЛОГИКА СИГНАЛОВ
   bool long_trend_signal = Crossover(quantum_osc_current, quantum_osc_prev, g_trend_long_entry);
   bool short_trend_signal = Crossunder(quantum_osc_current, quantum_osc_prev, g_trend_short_entry);
   bool long_range_signal = Crossover(quantum_osc_current, quantum_osc_prev, g_range_long_entry);
   bool short_range_signal = Crossunder(quantum_osc_current, quantum_osc_prev, g_range_short_entry);

   bool final_long_signal = (is_bullish_trend && long_trend_signal) || (is_volatile_range && long_range_signal);
   bool final_short_signal = (is_bearish_trend && short_trend_signal) || (is_volatile_range && short_range_signal);

//--- МОДУЛЬ 5: АВТОТОРГОВЛЯ
   if(PositionSelect(_Symbol) == false) // No open positions
     {
      double lot = CalculateLotSize();
      double sl = 0, tp = 0;

      if(final_long_signal)
        {
         MqlTick tick;
         SymbolInfoTick(_Symbol, tick);
         if(g_use_sl) sl = tick.ask - g_sl_pct/100.0 * tick.ask;
         if(g_use_tp) tp = tick.ask + g_tp_pct/100.0 * tick.ask;
         trade.Buy(lot, _Symbol, tick.ask, sl, tp, "Quantum Oscillator Long");
        }
      else if(final_short_signal)
        {
         MqlTick tick;
         SymbolInfoTick(_Symbol, tick);
         if(g_use_sl) sl = tick.bid + g_sl_pct/100.0 * tick.bid;
         if(g_use_tp) tp = tick.bid - g_tp_pct/100.0 * tick.bid;
         trade.Sell(lot, _Symbol, tick.bid, sl, tp, "Quantum Oscillator Short");
        }
     }
  }
//+------------------------------------------------------------------+
