﻿using DF.DB.DBModel;
using DF.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DF.DB
{
    public static class Members
    {
        public static ApiTableResponseModel<MemberModel> GetFilteredMembers(MemberFilterModel filter)
        {
            ApiTableResponseModel<MemberModel> response = new ApiTableResponseModel<MemberModel>();

            if (filter != null)
            {
                using (var ctx = new DFAppEntities())
                {
                    var q = ctx.Members
                                .Include(t => t.ChoreographyMembers)
                                .Include(t => t.DanceGroupMembers)
                                .Include(t => t.DanceSelectionMembers)
                                .Include(t => t.ContactData)
                                .Include("Performances.Events")
                                .AsQueryable();

                    bool excludeNonActive = true;
                    if (filter.ExcludeNonActive.HasValue)
                    {
                        excludeNonActive = (bool)filter.ExcludeNonActive;
                    }

                    if (excludeNonActive)
                    {
                        q = q.Where(x => x.IsActive);
                    }

                    if (!string.IsNullOrEmpty(filter.FullName))
                    {
                        q = q.Where(x => (x.FirstName.ToLower() + " " + x.LastName.ToLower()).Contains(filter.FullName.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(filter.JMBG))
                    {
                        q = q.Where(x => x.JMBG.StartsWith(filter.JMBG));
                    }

                    if (filter.ChoreoID.HasValue)
                    {
                        q = q.Where(x => x.ChoreographyMembers.Select(cm => cm.ChoreographyID).ToList().Contains((int)filter.ChoreoID));
                    }

                    if (filter.DanceGroupID.HasValue)
                    {
                        q = q.Where(x => x.DanceGroupMembers.Select(dgm => dgm.DanceGroupID).ToList().Contains((int)filter.DanceGroupID));
                    }

                    if (filter.DanceSelectionID.HasValue)
                    {
                        q = q.Where(x => x.DanceSelectionMembers.Select(dsm => dsm.DanceSelectionID).ToList().Contains((int)filter.DanceSelectionID));
                    }

                    if (filter.EventID.HasValue)
                    {
                        q = q.Where(x => x.Performances.Select(p => p.EventID).ToList().Contains((int)filter.EventID));
                    }

                    // paging & sorting
                    if (string.IsNullOrEmpty(filter.OrderByClause))
                    {
                        // default order
                        filter.OrderByClause = "FullName";
                    }

                    if (filter.PageNo < 1)
                    {
                        filter.PageNo = 1;
                    }

                    if (filter.RecordsPerPage < 1)
                    {
                        // unlimited
                        filter.RecordsPerPage = 1000000;
                    }

                    response.Total = q.Count();

                    var Data =
                        q.ToList()
                            .Select(x =>
                                new MemberModel()
                                {
                                    MemberID = x.MemberID,
                                    FirstName = x.FirstName,
                                    LastName = x.LastName,
                                    FullName = x.FirstName + " " + x.LastName,
                                    IsActive = x.IsActive,
                                    JMBG = x.JMBG,
                                    BirthDate = x.BirthDate,
                                    BirthPlace = x.BirthPlace,
                                    
                                    ContactData =
                                        new ContactDataModel()
                                        {
                                            Address = x.ContactData.Address,
                                            Email = x.ContactData.Email,
                                            Phone1 = x.ContactData.Phone1,
                                            Phone2 = x.ContactData.Phone2,
                                            Phone3 = x.ContactData.Phone3
                                        }
                                }
                            )
                            .OrderBy(filter.OrderByClause)
                            .Skip((filter.PageNo - 1) * filter.RecordsPerPage)
                            .Take(filter.RecordsPerPage);

                    response.Data = Data;
                }
            }

            return response;
        }

        public static void CreateMember(MemberModel model)
        {
            using (var ctx = new DFAppEntities())
            {
                var existingJMBG = ctx.Members.FirstOrDefault(x => x.JMBG == model.JMBG);
                if (existingJMBG != null)
                {
                    throw new Exception("error_members_jmbg_exists");
                }

                DBModel.Members m = new DBModel.Members();
                m.BirthDate = model.BirthDate;
                m.BirthPlace = model.BirthPlace;
                m.FirstName = model.FirstName;
                m.LastName = model.LastName;
                m.JMBG = model.JMBG;
                m.IsActive = true;

                m.ContactData = new ContactData();
                m.ContactData.Address = model.ContactData.Address;
                m.ContactData.Email = model.ContactData.Email;
                m.ContactData.Phone1 = model.ContactData.Phone1;
                m.ContactData.Phone2 = model.ContactData.Phone2;
                m.ContactData.Phone3 = model.ContactData.Phone3;

                ctx.Members.Add(m);
                ctx.SaveChanges();
            }
        }

        public static void EditMember(int id, MemberModel model)
        {
            using (var ctx = new DFAppEntities())
            {
                var existing = ctx.Members.FirstOrDefault(x => x.MemberID == id);
                if (existing != null)
                {
                    if (model.BirthDate.HasValue)
                    {
                        existing.BirthDate = model.BirthDate;
                    }

                    if (!string.IsNullOrEmpty(model.BirthPlace))
                    {
                        existing.BirthPlace = model.BirthPlace;
                    }

                    if (model.ContactData != null && !string.IsNullOrEmpty(model.ContactData.Address))
                    {
                        existing.ContactData.Address = model.ContactData.Address;
                    }

                    if (model.ContactData != null && !string.IsNullOrEmpty(model.ContactData.Email))
                    {
                        existing.ContactData.Email = model.ContactData.Email;
                    }

                    if (model.ContactData != null && !string.IsNullOrEmpty(model.ContactData.Address))
                    {
                        existing.ContactData.Address = model.ContactData.Address;
                    }

                    if (model.ContactData != null && !string.IsNullOrEmpty(model.ContactData.Phone1))
                    {
                        existing.ContactData.Phone1 = model.ContactData.Phone1;
                    }

                    if (model.ContactData != null && !string.IsNullOrEmpty(model.ContactData.Phone2))
                    {
                        existing.ContactData.Phone2 = model.ContactData.Phone2;
                    }

                    if (model.ContactData != null && !string.IsNullOrEmpty(model.ContactData.Phone3))
                    {
                        existing.ContactData.Phone3 = model.ContactData.Phone3;
                    }

                    if (model.IsActive.HasValue)
                    {
                        existing.IsActive = (bool)model.IsActive;
                    }

                    ctx.SaveChanges();
                }
            }
        }
    }
}
