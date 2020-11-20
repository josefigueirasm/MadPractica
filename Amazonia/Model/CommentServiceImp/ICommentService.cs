﻿using Es.Udc.DotNet.Amazonia.Model.DAOs.CommentDao;
using Es.Udc.DotNet.Amazonia.Model.DAOs.ProductDao;
using System.Collections.Generic;
using System;
using System.Management.Instrumentation;
using Es.Udc.DotNet.ModelUtil.Transactions;

namespace Es.Udc.DotNet.Amazonia.Model.CommentServiceImp
{
    public interface ICommentService
    {
        ICommentDao CommentDao { set; }
        IProductDao ProductDao { set; }

        /// <summary>
        /// Adds the comment.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="value">The value.</param>
        /// <param name="productId">The product identifier.</param>
        /// <exception cref="ArgumentNullException"/>

        /// <returns></returns>
        [Transactional]
        Comment AddComment(string title, string value, long productId);

        /// <summary>
        /// Finds the comments of product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <exception cref="InstanceNotFoundException"/> 
        /// <returns></returns>
        [Transactional]
        List<Comment> FindCommentsOfProduct(long productId);
    }
}