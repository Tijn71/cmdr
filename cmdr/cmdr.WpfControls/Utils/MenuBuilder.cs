﻿using cmdr.WpfControls.DropDownButton;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cmdr.WpfControls.Utils
{
    public class MenuBuilder<T>
    {
        public List<MenuItemViewModel> BuildList(IEnumerable<T> proxies, Func<T, MenuItemViewModel> proxyConverter)
        {
            return proxies.Select(p => proxyConverter(p)).OrderBy(m => m).ToList();
        }

        public List<MenuItemViewModel> BuildTree(IEnumerable<T> proxies, Func<T, MenuItemViewModel> proxyConverter, Func<T, string> pathSelector, 
            string pathSeparator, bool pathIncludesLeafs, bool sort=true)
        {
            var paths = proxies.Select(pathSelector);
            if (!pathIncludesLeafs)
                paths = paths.Distinct();

            IEnumerable<string> parts = null;
            int len;

            MenuItemViewModel root = new MenuItemViewModel { Text = "Root" };

            foreach (var p in paths)
            {
                parts = p.Split(new string[] { pathSeparator }, StringSplitOptions.RemoveEmptyEntries);
                len = parts.Count();
                parts = parts.Take(pathIncludesLeafs ? len - 1 : len);

                MenuItemViewModel target = root;
                foreach (var c in parts)
                {
                    if (!target.Children.Any(ch => ch.Text == c))
                        target.Children.Add(new MenuItemViewModel { Text = c });
                    target = target.Children.Single(ch => ch.Text == c);
                }

                target.Children.AddRange(BuildList(proxies.Where(i => pathSelector(i) == p), proxyConverter));
            }

            // pestrela: made an option to sort the commands in the Traktor order
            if(sort)
                sortTree(root);
            return root.Children;
        }


        private void sortTree(MenuItemViewModel tree)
        {
            var queue = new Queue<MenuItemViewModel>();
            queue.Enqueue(tree);

            while(queue.Any())
            {
                var el = queue.Dequeue();
                el.Children.Sort();
                el.Children.ForEach(c => queue.Enqueue(c));
            }
        }
    }
}
