using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace MAItems.Database
{
    public class FinancialRepository
    {
        private readonly DatabaseContext _context;

        public FinancialRepository(DatabaseContext context)
        {
            _context = context;
        }

        public CompanyProfile? GetCompanyProfile(long dealId)
        {
            using var conn = _context.GetConnection();
            using var cmd = new SqliteCommand("SELECT * FROM CompanyProfiles WHERE DealId = @DealId;", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapCompanyProfile(reader) : null;
        }

        public void UpsertCompanyProfile(CompanyProfile p)
        {
            using var conn = _context.GetConnection();
            string sql = @"INSERT INTO CompanyProfiles (DealId, CompanyName, CompanyNameSub, HeadOfficeAddress, FactoryAddress, OtherOffice, Founded, Founded2, Capital, RepresentativeName, RepresentativeProfile, ShareholderInfo, BusinessDetail, Revenue, Employees, MainClients, MainSuppliers, Certifications, GroupCompanies, TransferReason, Remarks) VALUES (@DealId, @CompanyName, @CompanyNameSub, @HeadOfficeAddress, @FactoryAddress, @OtherOffice, @Founded, @Founded2, @Capital, @RepresentativeName, @RepresentativeProfile, @ShareholderInfo, @BusinessDetail, @Revenue, @Employees, @MainClients, @MainSuppliers, @Certifications, @GroupCompanies, @TransferReason, @Remarks) ON CONFLICT(DealId) DO UPDATE SET CompanyName=excluded.CompanyName, CompanyNameSub=excluded.CompanyNameSub, HeadOfficeAddress=excluded.HeadOfficeAddress, FactoryAddress=excluded.FactoryAddress, OtherOffice=excluded.OtherOffice, Founded=excluded.Founded, Founded2=excluded.Founded2, Capital=excluded.Capital, RepresentativeName=excluded.RepresentativeName, RepresentativeProfile=excluded.RepresentativeProfile, ShareholderInfo=excluded.ShareholderInfo, BusinessDetail=excluded.BusinessDetail, Revenue=excluded.Revenue, Employees=excluded.Employees, MainClients=excluded.MainClients, MainSuppliers=excluded.MainSuppliers, Certifications=excluded.Certifications, GroupCompanies=excluded.GroupCompanies, TransferReason=excluded.TransferReason, Remarks=excluded.Remarks;";
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@DealId", p.DealId); cmd.Parameters.AddWithValue("@CompanyName", p.CompanyName); cmd.Parameters.AddWithValue("@CompanyNameSub", p.CompanyNameSub); cmd.Parameters.AddWithValue("@HeadOfficeAddress", p.HeadOfficeAddress); cmd.Parameters.AddWithValue("@FactoryAddress", p.FactoryAddress); cmd.Parameters.AddWithValue("@OtherOffice", p.OtherOffice); cmd.Parameters.AddWithValue("@Founded", p.Founded); cmd.Parameters.AddWithValue("@Founded2", p.Founded2); cmd.Parameters.AddWithValue("@Capital", p.Capital); cmd.Parameters.AddWithValue("@RepresentativeName", p.RepresentativeName); cmd.Parameters.AddWithValue("@RepresentativeProfile", p.RepresentativeProfile); cmd.Parameters.AddWithValue("@ShareholderInfo", p.ShareholderInfo); cmd.Parameters.AddWithValue("@BusinessDetail", p.BusinessDetail); cmd.Parameters.AddWithValue("@Revenue", p.Revenue); cmd.Parameters.AddWithValue("@Employees", p.Employees); cmd.Parameters.AddWithValue("@MainClients", p.MainClients); cmd.Parameters.AddWithValue("@MainSuppliers", p.MainSuppliers); cmd.Parameters.AddWithValue("@Certifications", p.Certifications); cmd.Parameters.AddWithValue("@GroupCompanies", p.GroupCompanies); cmd.Parameters.AddWithValue("@TransferReason", p.TransferReason); cmd.Parameters.AddWithValue("@Remarks", p.Remarks);
            cmd.ExecuteNonQuery();
        }

        public List<FinancialHighlight> GetFinancialHighlights(long dealId)
        {
            var list = new List<FinancialHighlight>();
            using var conn = _context.GetConnection();
            using var cmd = new SqliteCommand("SELECT * FROM FinancialHighlights WHERE DealId = @DealId ORDER BY PeriodType DESC, PeriodOrder ASC;", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(MapFinancialHighlight(reader));
            return list;
        }

        public void UpsertFinancialHighlight(FinancialHighlight f)
        {
            using var conn = _context.GetConnection();
            string sql = @"INSERT INTO FinancialHighlights (DealId, PeriodType, PeriodOrder, PeriodLabel, Revenue, CostRate, GrossProfit, GrossProfitRate, SGA, OperatingProfit, OperatingProfitRate, OrdinaryProfit, NetIncome, EBITDA, Depreciation, CapEx, CurrentAssets, CashEquivalents, AccountsReceivable, Inventory, OtherCurrentAssets, FixedAssets, TotalAssets, CurrentLiabilities, AccountsPayable, ShortTermDebt, OtherCurrentLiabilities, FixedLiabilities, LongTermDebt, OtherFixedLiabilities, TotalLiabilities, NetAssets, RetainedEarnings) VALUES (@DealId, @PeriodType, @PeriodOrder, @PeriodLabel, @Revenue, @CostRate, @GrossProfit, @GrossProfitRate, @SGA, @OperatingProfit, @OperatingProfitRate, @OrdinaryProfit, @NetIncome, @EBITDA, @Depreciation, @CapEx, @CurrentAssets, @CashEquivalents, @AccountsReceivable, @Inventory, @OtherCurrentAssets, @FixedAssets, @TotalAssets, @CurrentLiabilities, @AccountsPayable, @ShortTermDebt, @OtherCurrentLiabilities, @FixedLiabilities, @LongTermDebt, @OtherFixedLiabilities, @TotalLiabilities, @NetAssets, @RetainedEarnings) ON CONFLICT(DealId, PeriodType, PeriodOrder) DO UPDATE SET PeriodLabel=excluded.PeriodLabel, Revenue=excluded.Revenue, CostRate=excluded.CostRate, GrossProfit=excluded.GrossProfit, GrossProfitRate=excluded.GrossProfitRate, SGA=excluded.SGA, OperatingProfit=excluded.OperatingProfit, OperatingProfitRate=excluded.OperatingProfitRate, OrdinaryProfit=excluded.OrdinaryProfit, NetIncome=excluded.NetIncome, EBITDA=excluded.EBITDA, Depreciation=excluded.Depreciation, CapEx=excluded.CapEx, CurrentAssets=excluded.CurrentAssets, CashEquivalents=excluded.CashEquivalents, AccountsReceivable=excluded.AccountsReceivable, Inventory=excluded.Inventory, OtherCurrentAssets=excluded.OtherCurrentAssets, FixedAssets=excluded.FixedAssets, TotalAssets=excluded.TotalAssets, CurrentLiabilities=excluded.CurrentLiabilities, AccountsPayable=excluded.AccountsPayable, ShortTermDebt=excluded.ShortTermDebt, OtherCurrentLiabilities=excluded.OtherCurrentLiabilities, FixedLiabilities=excluded.FixedLiabilities, LongTermDebt=excluded.LongTermDebt, OtherFixedLiabilities=excluded.OtherFixedLiabilities, TotalLiabilities=excluded.TotalLiabilities, NetAssets=excluded.NetAssets, RetainedEarnings=excluded.RetainedEarnings;";
            using var cmd = new SqliteCommand(sql, conn);
            BindFinancialHighlight(cmd, f);
            cmd.ExecuteNonQuery();
        }

        public ValuationData? GetValuationData(long dealId)
        {
            using var conn = _context.GetConnection();
            using var cmd = new SqliteCommand("SELECT * FROM ValuationData WHERE DealId = @DealId;", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapValuationData(reader) : null;
        }

        public void UpsertValuationData(ValuationData v)
        {
            using var conn = _context.GetConnection();
            conn.Open(); // 念のためOpenを明示

            // ── 変更: INSERT文とUPDATE文に、さらに5つの新しい項目を追加 ──
            string sql = @"
                INSERT INTO ValuationData (
                    DealId, NetAssetValue, NetAssetNote, EBITDABase, EBITDABaseYear, EBITDAMultiple, EBITDANetCashDebt, EBITDANote, 
                    DCFDiscountRate, DCFTerminalGrowth, DCFEV, DCFNetCashDebt, DCFNote, NOI, CapRate, DirectNetCashDebt, DirectNote, 
                    EBITDAEquityValue, DCFEquityValue, DirectEquityValue, ValuationNote,
                    CashAndDeposits, MarketableSecurities, InsuranceReserves, OtherAssets, WorkingCapitalMonths,
                    ShortTermDebt, LongTermDebt, LeaseObligations, OtherLiabilities,
                    OpProfit_NA, TaxRate_NA, GoodwillYears, OpProfit_Direct, TaxRate_Direct
                ) VALUES (
                    @DealId, @NetAssetValue, @NetAssetNote, @EBITDABase, @EBITDABaseYear, @EBITDAMultiple, @EBITDANetCashDebt, @EBITDANote, 
                    @DCFDiscountRate, @DCFTerminalGrowth, @DCFEV, @DCFNetCashDebt, @DCFNote, @NOI, @CapRate, @DirectNetCashDebt, @DirectNote, 
                    @EBITDAEquityValue, @DCFEquityValue, @DirectEquityValue, @ValuationNote,
                    @CashAndDeposits, @MarketableSecurities, @InsuranceReserves, @OtherAssets, @WorkingCapitalMonths,
                    @ShortTermDebt, @LongTermDebt, @LeaseObligations, @OtherLiabilities,
                    @OpProfit_NA, @TaxRate_NA, @GoodwillYears, @OpProfit_Direct, @TaxRate_Direct
                ) ON CONFLICT(DealId) DO UPDATE SET 
                    NetAssetValue=excluded.NetAssetValue, NetAssetNote=excluded.NetAssetNote, EBITDABase=excluded.EBITDABase, 
                    EBITDABaseYear=excluded.EBITDABaseYear, EBITDAMultiple=excluded.EBITDAMultiple, EBITDANetCashDebt=excluded.EBITDANetCashDebt, 
                    EBITDANote=excluded.EBITDANote, DCFDiscountRate=excluded.DCFDiscountRate, DCFTerminalGrowth=excluded.DCFTerminalGrowth, 
                    DCFEV=excluded.DCFEV, DCFNetCashDebt=excluded.DCFNetCashDebt, DCFNote=excluded.DCFNote, NOI=excluded.NOI, 
                    CapRate=excluded.CapRate, DirectNetCashDebt=excluded.DirectNetCashDebt, DirectNote=excluded.DirectNote, 
                    EBITDAEquityValue=excluded.EBITDAEquityValue, DCFEquityValue=excluded.DCFEquityValue, DirectEquityValue=excluded.DirectEquityValue, 
                    ValuationNote=excluded.ValuationNote,
                    CashAndDeposits=excluded.CashAndDeposits, MarketableSecurities=excluded.MarketableSecurities, 
                    InsuranceReserves=excluded.InsuranceReserves, OtherAssets=excluded.OtherAssets, WorkingCapitalMonths=excluded.WorkingCapitalMonths,
                    ShortTermDebt=excluded.ShortTermDebt, LongTermDebt=excluded.LongTermDebt, LeaseObligations=excluded.LeaseObligations, OtherLiabilities=excluded.OtherLiabilities,
                    OpProfit_NA=excluded.OpProfit_NA, TaxRate_NA=excluded.TaxRate_NA, GoodwillYears=excluded.GoodwillYears, 
                    OpProfit_Direct=excluded.OpProfit_Direct, TaxRate_Direct=excluded.TaxRate_Direct;";

            using var cmd = new SqliteCommand(sql, conn);

            // 既存項目のバインド
            cmd.Parameters.AddWithValue("@DealId", v.DealId); cmd.Parameters.AddWithValue("@NetAssetNote", v.NetAssetNote ?? "");
            cmd.Parameters.AddWithValue("@EBITDABaseYear", v.EBITDABaseYear ?? ""); cmd.Parameters.AddWithValue("@EBITDANote", v.EBITDANote ?? "");
            cmd.Parameters.AddWithValue("@DCFNote", v.DCFNote ?? ""); cmd.Parameters.AddWithValue("@DirectNote", v.DirectNote ?? "");
            cmd.Parameters.AddWithValue("@ValuationNote", v.ValuationNote ?? "");
            DbHelperUtils.BindReal(cmd, "@NetAssetValue", v.NetAssetValue); DbHelperUtils.BindReal(cmd, "@EBITDABase", v.EBITDABase);
            DbHelperUtils.BindReal(cmd, "@EBITDAMultiple", v.EBITDAMultiple); DbHelperUtils.BindReal(cmd, "@EBITDANetCashDebt", v.EBITDANetCashDebt);
            DbHelperUtils.BindReal(cmd, "@DCFDiscountRate", v.DCFDiscountRate); DbHelperUtils.BindReal(cmd, "@DCFTerminalGrowth", v.DCFTerminalGrowth);
            DbHelperUtils.BindReal(cmd, "@DCFEV", v.DCFEV); DbHelperUtils.BindReal(cmd, "@DCFNetCashDebt", v.DCFNetCashDebt);
            DbHelperUtils.BindReal(cmd, "@NOI", v.NOI); DbHelperUtils.BindReal(cmd, "@CapRate", v.CapRate);
            DbHelperUtils.BindReal(cmd, "@DirectNetCashDebt", v.DirectNetCashDebt); DbHelperUtils.BindReal(cmd, "@EBITDAEquityValue", v.EBITDAEquityValue);
            DbHelperUtils.BindReal(cmd, "@DCFEquityValue", v.DCFEquityValue); DbHelperUtils.BindReal(cmd, "@DirectEquityValue", v.DirectEquityValue);

            // 追加項目のバインド
            DbHelperUtils.BindReal(cmd, "@CashAndDeposits", v.CashAndDeposits);
            DbHelperUtils.BindReal(cmd, "@MarketableSecurities", v.MarketableSecurities);
            DbHelperUtils.BindReal(cmd, "@InsuranceReserves", v.InsuranceReserves);
            DbHelperUtils.BindReal(cmd, "@OtherAssets", v.OtherAssets);
            DbHelperUtils.BindReal(cmd, "@WorkingCapitalMonths", v.WorkingCapitalMonths);
            DbHelperUtils.BindReal(cmd, "@ShortTermDebt", v.ShortTermDebt);
            DbHelperUtils.BindReal(cmd, "@LongTermDebt", v.LongTermDebt);
            DbHelperUtils.BindReal(cmd, "@LeaseObligations", v.LeaseObligations);
            DbHelperUtils.BindReal(cmd, "@OtherLiabilities", v.OtherLiabilities);

            // ── ★ さらに追加: 新たに増えた5つの項目のバインド ──
            DbHelperUtils.BindReal(cmd, "@OpProfit_NA", v.OpProfit_NA);
            DbHelperUtils.BindReal(cmd, "@TaxRate_NA", v.TaxRate_NA);
            DbHelperUtils.BindReal(cmd, "@GoodwillYears", v.GoodwillYears);
            DbHelperUtils.BindReal(cmd, "@OpProfit_Direct", v.OpProfit_Direct);
            DbHelperUtils.BindReal(cmd, "@TaxRate_Direct", v.TaxRate_Direct);

            cmd.ExecuteNonQuery();
        }

        // ══════════════════════════════════════════════════════
        // 純資産法 修正項目 (NetAssetAdjustments) の処理
        // ══════════════════════════════════════════════════════


        public void SaveNetAssetAdjustments(long dealId, List<NetAssetAdjustment> adjustments)
        {
            using var conn = _context.GetConnection();
            using var tx = conn.BeginTransaction();

            // 古いデータを一度クリア
            using (var del = new SqliteCommand("DELETE FROM NetAssetAdjustments WHERE DealId = @DealId;", conn, tx))
            {
                del.Parameters.AddWithValue("@DealId", dealId);
                del.ExecuteNonQuery();
            }

            // 新しいデータを一括登録
            string sql = "INSERT INTO NetAssetAdjustments (DealId, AdjustType, ItemName, Amount, Remarks) VALUES (@DealId, @AdjustType, @ItemName, @Amount, @Remarks);";
            foreach (var a in adjustments)
            {
                using var cmd = new SqliteCommand(sql, conn, tx);
                cmd.Parameters.AddWithValue("@DealId", dealId); cmd.Parameters.AddWithValue("@AdjustType", a.AdjustType);
                cmd.Parameters.AddWithValue("@ItemName", a.ItemName ?? ""); cmd.Parameters.AddWithValue("@Amount", a.Amount); cmd.Parameters.AddWithValue("@Remarks", a.Remarks ?? "");
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }

        // ══════════════════════════════════════════════════════
        // DCF法 将来計画 (DcfProjections) の処理
        // ══════════════════════════════════════════════════════


        public void SaveDcfProjections(long dealId, List<DcfProjection> projections)
        {
            using var conn = _context.GetConnection();
            using var tx = conn.BeginTransaction();
            using (var del = new SqliteCommand("DELETE FROM DcfProjections WHERE DealId = @DealId;", conn, tx))
            {
                del.Parameters.AddWithValue("@DealId", dealId); del.ExecuteNonQuery();
            }

            string sql = "INSERT INTO DcfProjections (DealId, YearIndex, Revenue, OpProfit, TaxRate, DiscountRate, TerminalGrowth) VALUES (@DealId, @YearIndex, @Revenue, @OpProfit, @TaxRate, @DiscountRate, @TerminalGrowth);";
            foreach (var p in projections)
            {
                using var cmd = new SqliteCommand(sql, conn, tx);
                cmd.Parameters.AddWithValue("@DealId", dealId); cmd.Parameters.AddWithValue("@YearIndex", p.YearIndex);
                DbHelperUtils.BindReal(cmd, "@Revenue", p.Revenue); DbHelperUtils.BindReal(cmd, "@OpProfit", p.OpProfit); DbHelperUtils.BindReal(cmd, "@TaxRate", p.TaxRate); DbHelperUtils.BindReal(cmd, "@DiscountRate", p.DiscountRate); DbHelperUtils.BindReal(cmd, "@TerminalGrowth", p.TerminalGrowth);
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }



        private static CompanyProfile MapCompanyProfile(SqliteDataReader r) => new CompanyProfile { Id = r.GetInt64(r.GetOrdinal("Id")), DealId = r.GetInt64(r.GetOrdinal("DealId")), CompanyName = DbHelperUtils.StrD(r, "CompanyName"), CompanyNameSub = DbHelperUtils.StrD(r, "CompanyNameSub"), HeadOfficeAddress = DbHelperUtils.StrD(r, "HeadOfficeAddress"), FactoryAddress = DbHelperUtils.StrD(r, "FactoryAddress"), OtherOffice = DbHelperUtils.StrD(r, "OtherOffice"), Founded = DbHelperUtils.StrD(r, "Founded"), Founded2 = DbHelperUtils.StrD(r, "Founded2"), Capital = DbHelperUtils.StrD(r, "Capital"), RepresentativeName = DbHelperUtils.StrD(r, "RepresentativeName"), RepresentativeProfile = DbHelperUtils.StrD(r, "RepresentativeProfile"), ShareholderInfo = DbHelperUtils.StrD(r, "ShareholderInfo"), BusinessDetail = DbHelperUtils.StrD(r, "BusinessDetail"), Revenue = DbHelperUtils.StrD(r, "Revenue"), Employees = DbHelperUtils.StrD(r, "Employees"), MainClients = DbHelperUtils.StrD(r, "MainClients"), MainSuppliers = DbHelperUtils.StrD(r, "MainSuppliers"), Certifications = DbHelperUtils.StrD(r, "Certifications"), GroupCompanies = DbHelperUtils.StrD(r, "GroupCompanies"), TransferReason = DbHelperUtils.StrD(r, "TransferReason"), Remarks = DbHelperUtils.StrD(r, "Remarks") };
        private static FinancialHighlight MapFinancialHighlight(SqliteDataReader r) => new FinancialHighlight { Id = r.GetInt64(r.GetOrdinal("Id")), DealId = r.GetInt64(r.GetOrdinal("DealId")), PeriodType = DbHelperUtils.StrD(r, "PeriodType"), PeriodOrder = r.GetInt32(r.GetOrdinal("PeriodOrder")), PeriodLabel = DbHelperUtils.StrD(r, "PeriodLabel"), Revenue = DbHelperUtils.RealN(r, "Revenue"), CostRate = DbHelperUtils.RealN(r, "CostRate"), GrossProfit = DbHelperUtils.RealN(r, "GrossProfit"), GrossProfitRate = DbHelperUtils.RealN(r, "GrossProfitRate"), SGA = DbHelperUtils.RealN(r, "SGA"), OperatingProfit = DbHelperUtils.RealN(r, "OperatingProfit"), OperatingProfitRate = DbHelperUtils.RealN(r, "OperatingProfitRate"), OrdinaryProfit = DbHelperUtils.RealN(r, "OrdinaryProfit"), NetIncome = DbHelperUtils.RealN(r, "NetIncome"), EBITDA = DbHelperUtils.RealN(r, "EBITDA"), Depreciation = DbHelperUtils.RealN(r, "Depreciation"), CapEx = DbHelperUtils.RealN(r, "CapEx"), CurrentAssets = DbHelperUtils.RealN(r, "CurrentAssets"), CashEquivalents = DbHelperUtils.RealN(r, "CashEquivalents"), AccountsReceivable = DbHelperUtils.RealN(r, "AccountsReceivable"), Inventory = DbHelperUtils.RealN(r, "Inventory"), OtherCurrentAssets = DbHelperUtils.RealN(r, "OtherCurrentAssets"), FixedAssets = DbHelperUtils.RealN(r, "FixedAssets"), TotalAssets = DbHelperUtils.RealN(r, "TotalAssets"), CurrentLiabilities = DbHelperUtils.RealN(r, "CurrentLiabilities"), AccountsPayable = DbHelperUtils.RealN(r, "AccountsPayable"), ShortTermDebt = DbHelperUtils.RealN(r, "ShortTermDebt"), OtherCurrentLiabilities = DbHelperUtils.RealN(r, "OtherCurrentLiabilities"), FixedLiabilities = DbHelperUtils.RealN(r, "FixedLiabilities"), LongTermDebt = DbHelperUtils.RealN(r, "LongTermDebt"), OtherFixedLiabilities = DbHelperUtils.RealN(r, "OtherFixedLiabilities"), TotalLiabilities = DbHelperUtils.RealN(r, "TotalLiabilities"), NetAssets = DbHelperUtils.RealN(r, "NetAssets"), RetainedEarnings = DbHelperUtils.RealN(r, "RetainedEarnings") };
        private static ValuationData MapValuationData(SqliteDataReader r)
        {
            var v = new ValuationData
            {
                Id = r.GetInt64(r.GetOrdinal("Id")),
                DealId = r.GetInt64(r.GetOrdinal("DealId")),
                NetAssetValue = DbHelperUtils.RealN(r, "NetAssetValue"),
                NetAssetNote = DbHelperUtils.StrD(r, "NetAssetNote"),
                EBITDABase = DbHelperUtils.RealN(r, "EBITDABase"),
                EBITDABaseYear = DbHelperUtils.StrD(r, "EBITDABaseYear"),
                EBITDAMultiple = DbHelperUtils.RealN(r, "EBITDAMultiple"),
                EBITDANetCashDebt = DbHelperUtils.RealN(r, "EBITDANetCashDebt"),
                EBITDANote = DbHelperUtils.StrD(r, "EBITDANote"),
                DCFDiscountRate = DbHelperUtils.RealN(r, "DCFDiscountRate"),
                DCFTerminalGrowth = DbHelperUtils.RealN(r, "DCFTerminalGrowth"),
                DCFEV = DbHelperUtils.RealN(r, "DCFEV"),
                DCFNetCashDebt = DbHelperUtils.RealN(r, "DCFNetCashDebt"),
                DCFNote = DbHelperUtils.StrD(r, "DCFNote"),
                NOI = DbHelperUtils.RealN(r, "NOI"),
                CapRate = DbHelperUtils.RealN(r, "CapRate"),
                DirectNetCashDebt = DbHelperUtils.RealN(r, "DirectNetCashDebt"),
                DirectNote = DbHelperUtils.StrD(r, "DirectNote"),
                EBITDAEquityValue = DbHelperUtils.RealN(r, "EBITDAEquityValue"),
                DCFEquityValue = DbHelperUtils.RealN(r, "DCFEquityValue"),
                DirectEquityValue = DbHelperUtils.RealN(r, "DirectEquityValue"),
                ValuationNote = DbHelperUtils.StrD(r, "ValuationNote"),

                // ── 既存の追加項目 ──
                CashAndDeposits = DbHelperUtils.HasColumn(r, "CashAndDeposits") ? DbHelperUtils.RealN(r, "CashAndDeposits") : null,
                MarketableSecurities = DbHelperUtils.HasColumn(r, "MarketableSecurities") ? DbHelperUtils.RealN(r, "MarketableSecurities") : null,
                InsuranceReserves = DbHelperUtils.HasColumn(r, "InsuranceReserves") ? DbHelperUtils.RealN(r, "InsuranceReserves") : null,
                OtherAssets = DbHelperUtils.HasColumn(r, "OtherAssets") ? DbHelperUtils.RealN(r, "OtherAssets") : null,
                WorkingCapitalMonths = DbHelperUtils.HasColumn(r, "WorkingCapitalMonths") ? DbHelperUtils.RealN(r, "WorkingCapitalMonths") : null,
                ShortTermDebt = DbHelperUtils.HasColumn(r, "ShortTermDebt") ? DbHelperUtils.RealN(r, "ShortTermDebt") : null,
                LongTermDebt = DbHelperUtils.HasColumn(r, "LongTermDebt") ? DbHelperUtils.RealN(r, "LongTermDebt") : null,
                LeaseObligations = DbHelperUtils.HasColumn(r, "LeaseObligations") ? DbHelperUtils.RealN(r, "LeaseObligations") : null,
                OtherLiabilities = DbHelperUtils.HasColumn(r, "OtherLiabilities") ? DbHelperUtils.RealN(r, "OtherLiabilities") : null,

                // ── 💡【重要・ここを追加】新UI用の残りの5項目もDBから確実に読み出す ──
                OpProfit_NA = DbHelperUtils.HasColumn(r, "OpProfit_NA") ? DbHelperUtils.RealN(r, "OpProfit_NA") : null,
                TaxRate_NA = DbHelperUtils.HasColumn(r, "TaxRate_NA") ? DbHelperUtils.RealN(r, "TaxRate_NA") : null,
                GoodwillYears = DbHelperUtils.HasColumn(r, "GoodwillYears") ? DbHelperUtils.RealN(r, "GoodwillYears") : null,
                OpProfit_Direct = DbHelperUtils.HasColumn(r, "OpProfit_Direct") ? DbHelperUtils.RealN(r, "OpProfit_Direct") : null,
                TaxRate_Direct = DbHelperUtils.HasColumn(r, "TaxRate_Direct") ? DbHelperUtils.RealN(r, "TaxRate_Direct") : null
            };

            return v;
        }


        private static void BindFinancialHighlight(SqliteCommand cmd, FinancialHighlight f)
        {
            cmd.Parameters.AddWithValue("@DealId", f.DealId); cmd.Parameters.AddWithValue("@PeriodType", f.PeriodType); cmd.Parameters.AddWithValue("@PeriodOrder", f.PeriodOrder); cmd.Parameters.AddWithValue("@PeriodLabel", f.PeriodLabel ?? "");
            DbHelperUtils.BindReal(cmd, "@Revenue", f.Revenue); DbHelperUtils.BindReal(cmd, "@CostRate", f.CostRate); DbHelperUtils.BindReal(cmd, "@GrossProfit", f.GrossProfit); DbHelperUtils.BindReal(cmd, "@GrossProfitRate", f.GrossProfitRate); DbHelperUtils.BindReal(cmd, "@SGA", f.SGA); DbHelperUtils.BindReal(cmd, "@OperatingProfit", f.OperatingProfit); DbHelperUtils.BindReal(cmd, "@OperatingProfitRate", f.OperatingProfitRate); DbHelperUtils.BindReal(cmd, "@OrdinaryProfit", f.OrdinaryProfit); DbHelperUtils.BindReal(cmd, "@NetIncome", f.NetIncome); DbHelperUtils.BindReal(cmd, "@EBITDA", f.EBITDA); DbHelperUtils.BindReal(cmd, "@Depreciation", f.Depreciation); DbHelperUtils.BindReal(cmd, "@CapEx", f.CapEx); DbHelperUtils.BindReal(cmd, "@CurrentAssets", f.CurrentAssets); DbHelperUtils.BindReal(cmd, "@CashEquivalents", f.CashEquivalents); DbHelperUtils.BindReal(cmd, "@AccountsReceivable", f.AccountsReceivable); DbHelperUtils.BindReal(cmd, "@Inventory", f.Inventory); DbHelperUtils.BindReal(cmd, "@OtherCurrentAssets", f.OtherCurrentAssets); DbHelperUtils.BindReal(cmd, "@FixedAssets", f.FixedAssets); DbHelperUtils.BindReal(cmd, "@TotalAssets", f.TotalAssets); DbHelperUtils.BindReal(cmd, "@CurrentLiabilities", f.CurrentLiabilities); DbHelperUtils.BindReal(cmd, "@AccountsPayable", f.AccountsPayable); DbHelperUtils.BindReal(cmd, "@ShortTermDebt", f.ShortTermDebt); DbHelperUtils.BindReal(cmd, "@OtherCurrentLiabilities", f.OtherCurrentLiabilities); DbHelperUtils.BindReal(cmd, "@FixedLiabilities", f.FixedLiabilities); DbHelperUtils.BindReal(cmd, "@LongTermDebt", f.LongTermDebt); DbHelperUtils.BindReal(cmd, "@OtherFixedLiabilities", f.OtherFixedLiabilities); DbHelperUtils.BindReal(cmd, "@TotalLiabilities", f.TotalLiabilities); DbHelperUtils.BindReal(cmd, "@NetAssets", f.NetAssets); DbHelperUtils.BindReal(cmd, "@RetainedEarnings", f.RetainedEarnings);
        }

        // ════════════════════════════════════════════════
        // ① 純資産法の調整項目 (NetAssetAdjustments) の操作
        // ════════════════════════════════════════════════
        public void DeleteNetAssetAdjustments(long dealId)
        {
            using var conn = _context.GetConnection(); // ※適宜、ご自身のDB接続メソッドに合わせてください(例: new SqliteConnection(...))
            conn.Open();
            using var cmd = new SqliteCommand("DELETE FROM NetAssetAdjustments WHERE DealId = @DealId", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            cmd.ExecuteNonQuery();
        }

        public void AddNetAssetAdjustment(NetAssetAdjustment adj)
        {
            using var conn = _context.GetConnection();
            conn.Open();
            using var cmd = new SqliteCommand("INSERT INTO NetAssetAdjustments (DealId, AdjustType, ItemName, Amount, Remarks) VALUES (@DealId, @AdjustType, @ItemName, @Amount, @Remarks)", conn);
            cmd.Parameters.AddWithValue("@DealId", adj.DealId);
            cmd.Parameters.AddWithValue("@AdjustType", adj.AdjustType);
            cmd.Parameters.AddWithValue("@ItemName", adj.ItemName ?? "");
            cmd.Parameters.AddWithValue("@Amount", adj.Amount);
            cmd.Parameters.AddWithValue("@Remarks", adj.Remarks ?? "");
            cmd.ExecuteNonQuery();
        }

        public List<NetAssetAdjustment> GetNetAssetAdjustments(long dealId)
        {
            var list = new List<NetAssetAdjustment>();
            using var conn = _context.GetConnection();
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM NetAssetAdjustments WHERE DealId = @DealId ORDER BY AdjustType, Id", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new NetAssetAdjustment
                {
                    Id = Convert.ToInt64(reader["Id"]),
                    DealId = Convert.ToInt64(reader["DealId"]),
                    AdjustType = Convert.ToInt32(reader["AdjustType"]),
                    ItemName = reader["ItemName"]?.ToString() ?? string.Empty,
                    Amount = Convert.ToDouble(reader["Amount"]),
                    Remarks = reader["Remarks"]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        // ════════════════════════════════════════════════
        // ② DCF法の将来計画 (DcfProjections) の操作
        // ════════════════════════════════════════════════
        public void DeleteDcfProjections(long dealId)
        {
            using var conn = _context.GetConnection();
            conn.Open();
            using var cmd = new SqliteCommand("DELETE FROM DcfProjections WHERE DealId = @DealId", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            cmd.ExecuteNonQuery();
        }

        public void AddDcfProjection(DcfProjection proj)
        {
            using var conn = _context.GetConnection();
            conn.Open();
            using var cmd = new SqliteCommand("INSERT INTO DcfProjections (DealId, YearIndex, Revenue, OpProfit, TaxRate, DiscountRate, TerminalGrowth) VALUES (@DealId, @YearIndex, @Revenue, @OpProfit, @TaxRate, @DiscountRate, @TerminalGrowth)", conn);
            cmd.Parameters.AddWithValue("@DealId", proj.DealId);
            cmd.Parameters.AddWithValue("@YearIndex", proj.YearIndex);
            cmd.Parameters.AddWithValue("@Revenue", proj.Revenue.HasValue ? (object)proj.Revenue.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@OpProfit", proj.OpProfit.HasValue ? (object)proj.OpProfit.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@TaxRate", proj.TaxRate.HasValue ? (object)proj.TaxRate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@DiscountRate", proj.DiscountRate.HasValue ? (object)proj.DiscountRate.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@TerminalGrowth", proj.TerminalGrowth.HasValue ? (object)proj.TerminalGrowth.Value : DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public List<DcfProjection> GetDcfProjections(long dealId)
        {
            var list = new List<DcfProjection>();
            using var conn = _context.GetConnection();
            conn.Open();
            using var cmd = new SqliteCommand("SELECT * FROM DcfProjections WHERE DealId = @DealId ORDER BY YearIndex ASC", conn);
            cmd.Parameters.AddWithValue("@DealId", dealId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new DcfProjection
                {
                    Id = Convert.ToInt64(reader["Id"]),
                    DealId = Convert.ToInt64(reader["DealId"]),
                    YearIndex = Convert.ToInt32(reader["YearIndex"]),
                    Revenue = reader["Revenue"] != DBNull.Value ? Convert.ToDouble(reader["Revenue"]) : null,
                    OpProfit = reader["OpProfit"] != DBNull.Value ? Convert.ToDouble(reader["OpProfit"]) : null,
                    TaxRate = reader["TaxRate"] != DBNull.Value ? Convert.ToDouble(reader["TaxRate"]) : null,
                    DiscountRate = reader["DiscountRate"] != DBNull.Value ? Convert.ToDouble(reader["DiscountRate"]) : null,
                    TerminalGrowth = reader["TerminalGrowth"] != DBNull.Value ? Convert.ToDouble(reader["TerminalGrowth"]) : null
                });
            }
            return list;
        }



    }
}